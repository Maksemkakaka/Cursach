using UnityEngine;

public class FirstPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f; // Скорость поворота
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f; // Чувствительность мыши
    private Rigidbody rb;
    private bool isGrounded;
    public GameObject holdPoint; // Точка захвата на камере
    private GameObject heldObject; // Ссылка на объект, который держим в данный момент
    public Camera playerCamera;
    private bool isClimbing = false;
    private float xRotation = 0f; // Для сохранения поворота камеры по оси X
    public float climbSpeed = 5f;
    public float knockbackForce = 1.5f;
    private Animator animator; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Перемещение влево/вправо
        float moveHorizontal = Input.GetAxis("Horizontal");
        // Перемещение вперёд/назад
        float moveVertical = Input.GetAxis("Vertical");
        // Применяем движение
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
        if (moveVertical > 0)
        { // Если игрок движется вперед
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("isRunning", false); 
            animator.SetBool("isClimbing", false); 
        }
        else
        {
            animator.SetBool("Idle", false); // Если персонаж движется, он не в состоянии простоя
            if (moveVertical > 0)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }

        animator.SetBool("Grounded", isGrounded);
        // Вращение персонажа с помощью мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY; // Обновляем поворот по вертикали
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Ограничиваем вращение по оси X

        // Применяем повороты к камере и персонажу
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (Input.GetMouseButtonDown(0) && heldObject != null && heldObject.CompareTag("FireExtinguisher"))
        {
            // Активируем партикл-систему
            ToggleExtinguisherParticle(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Деактивируем партикл-систему
            ToggleExtinguisherParticle(false);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            isGrounded = false;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if (isClimbing)
        {
            Climb(); // Включаем метод для взбирания
        }

        if (Input.GetMouseButtonDown(0) && heldObject != null && heldObject.CompareTag("HoseTag"))
        {
            ParticleSystem waterSpray = heldObject.GetComponentInChildren<ParticleSystem>();
            if (waterSpray != null)
            {
                waterSpray.Play();
            }
        }
        if (Input.GetMouseButtonUp(0) && heldObject != null && heldObject.CompareTag("HoseTag"))
        {
            ParticleSystem waterSpray = heldObject.GetComponentInChildren<ParticleSystem>();
            if (waterSpray != null)
            {
                waterSpray.Stop();
            }
        }
    }

    public void GetHit(Vector3 direction)
    {
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }

    void Interact()
    {
        if (heldObject != null && !heldObject.CompareTag("Ladder"))
        {
            ReleaseObject();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("FireExtinguisher"))
            {
                animator.SetTrigger("PickUp");
            }
        }
    }

    public void PickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("FireExtinguisher"))
            {
                GrabObject(hit.collider.gameObject);
            }
        }
    }


    void GrabObject(GameObject obj)
    {
        heldObject = obj;
        heldObject.transform.position = holdPoint.transform.position;
        heldObject.transform.rotation = holdPoint.transform.rotation;
        heldObject.transform.parent = holdPoint.transform;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
    }


    void ReleaseObject()
    {
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.transform.parent = null;
        heldObject = null;
    }

    void ToggleExtinguisherParticle(bool isActive)
    {
        ParticleSystem extinguisherParticle = heldObject.GetComponentInChildren<ParticleSystem>();
        if (extinguisherParticle != null)
        {
            if (isActive)
                extinguisherParticle.Play();
            else
                extinguisherParticle.Stop();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true; // Включаем режим взбирания
            rb.isKinematic = true; // Отключаем физику, чтобы не мешала взбираться
            animator.SetBool("isClimbing", true); // Включаем анимацию взбирания
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false; // Выключаем режим взбирания
            rb.isKinematic = false; // Возвращаем управление физикой
            animator.SetBool("isClimbing", false); // Выключаем анимацию взбирания
        }
    }

    void Climb()
    {
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * verticalInput * climbSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Grounded", isGrounded); // Обновите Animator при приземлении
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Grounded", isGrounded); // Обновите Animator при отрыве от земли
        }
    }
}
