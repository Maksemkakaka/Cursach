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
    private Hose hoseComponent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Заблокировать курсор в центре экрана
        hoseComponent = FindObjectOfType<Hose>();
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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if (isClimbing)
        {
            Climb();
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

        void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void Interact()
    {
        // Если в руках уже есть объект, мы его отпускаем
        if (heldObject != null)
        {
            ReleaseObject();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // Добавьте эту строку для логирования

            if (hit.collider.CompareTag("FireExtinguisher") || hit.collider.CompareTag("Ladder"))
            {
                GrabObject(hit.collider.gameObject);
            }
        }
    }


    void GrabObject(GameObject obj)
    {
        heldObject = obj;
        // Устанавливаем объект точно на позицию точки захвата
        heldObject.transform.position = holdPoint.transform.position;
        // Поворачиваем объект так, чтобы он смотрел в том же направлении, что и точка захвата
        // Здесь можно установить желаемый поворот, например, (0,0,0) для стандартного выравнивания
        heldObject.transform.rotation = holdPoint.transform.rotation;
        // Делаем объект дочерним элементом точки захвата, чтобы он следовал за движениями игрока
        heldObject.transform.parent = holdPoint.transform;
        // Отключаем физику объекта, чтобы он не падал, пока мы его держим
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
    }


    void ReleaseObject()
    {
        // Возвращаем физику объекта
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        // Убираем родительскую связь
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
            isClimbing = true;
            rb.isKinematic = true; // Отключаем физику, чтобы предотвратить падение
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.isKinematic = false;
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
        }
    }
}
