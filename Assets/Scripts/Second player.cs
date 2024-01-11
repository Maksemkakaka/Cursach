using UnityEngine;

public class SecondPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f; // Скорость поворота
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    public Camera playerCamera;
    public LayerMask enemyLayer;
    public GameObject holdPoint; // Точка захвата на камере
    private GameObject heldObject; // Ссылка на объект, который держим в данный момент
    private Animator animator; 
    public GameObject swordPrefab;
    public float attackSphereRadius = 2f; // Радиус сферы атаки


    // Переменные для управления вращением персонажа
    private float pitch = 0f; // Угол наклона вверх-вниз
    public float pitchSpeed = 100f; // Скорость наклона камеры
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();

        // Получаем ввод с правого стика для поворота персонажа
        float turnAmount = Input.GetAxis("RightHorizontal") * turnSpeed * Time.deltaTime;
        // Получаем ввод с правого стика для наклона камеры вверх-вниз
        float pitchAmount = Input.GetAxis("RightVertical") * pitchSpeed * Time.deltaTime;

        // Поворачиваем персонажа
        transform.Rotate(0, turnAmount, 0);

        pitch = Mathf.Clamp(pitch - pitchAmount, -45f, 45f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, playerCamera.transform.localEulerAngles.y, 0);

        if (Input.GetKeyDown(KeyCode.JoystickButton3) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0)) 
        {

            if (heldObject != null && heldObject.CompareTag("Sword"))
            {
                
                DropSword();
            }
            else
            {
                
                Interact();
            }
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1) && heldObject != null && heldObject.CompareTag("Sword"))
        {
           
            animator.SetTrigger("Punch");
        }
    }
    void DropSword()
    {
        // Сброс меча и уничтожение объекта
        if (heldObject != null)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.transform.parent = null;
            Destroy(heldObject); // Уничтожить объект меча
            heldObject = null;
        }
    }
    public void CheckHit()
    {
        // Позиция для сферы атаки - перед камерой
        Vector3 spherePosition = playerCamera.transform.position + playerCamera.transform.forward * attackSphereRadius;

        // Визуализируем область атаки
        DrawDebugSphere(spherePosition, attackSphereRadius, Color.red, 0.5f);

        // Получаем всех врагов в сфере атаки
        Collider[] hitEnemies = Physics.OverlapSphere(spherePosition, attackSphereRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Проверяем, действительно ли объект - враг
            if (enemy.CompareTag("Enemy"))
            {
                // Наносим урон врагу
                enemy.GetComponent<Enemy>().TakeDamage(1);
            }
        }
    }

    private void DrawDebugSphere(Vector3 position, float radius, Color color, float duration)
    {
        Debug.DrawRay(position, Vector3.up * radius, color, duration);
        Debug.DrawRay(position, Vector3.down * radius, color, duration);
        Debug.DrawRay(position, Vector3.right * radius, color, duration);
        Debug.DrawRay(position, Vector3.left * radius, color, duration);
        Debug.DrawRay(position, Vector3.forward * radius, color, duration);
        Debug.DrawRay(position, Vector3.back * radius, color, duration);
    }


    void Move()
    {
        // Получаем ввод с левого джойстика
        float moveHorizontal = Input.GetAxis("Horizontal2");
        float moveVertical = Input.GetAxis("Vertical2");

        // Преобразуем направление ввода из локальных координат в мировые, используя направление взгляда камеры
        Vector3 movement = playerCamera.transform.right * moveHorizontal + playerCamera.transform.forward * moveVertical;
        movement.y = 0; // Игнорируем вертикальную составляющую, чтобы персонаж не летел в воздухе

        // Применяем движение
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);

        // Устанавливаем параметры анимации
        animator.SetBool("isRunning", movement.magnitude > 0);
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        animator.SetTrigger("Jump"); // Активируем анимацию прыжка
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
            // Персонаж может поднимать только лестницу
            if (hit.collider.CompareTag("Ladder"))
            {
                GrabObject(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Chest"))
            {
                // Взаимодействие с ящиком для поднятия меча
                animator.SetTrigger("PickUp");
            }
        }
        
    }

    void GrabObject(GameObject obj)
    {
        heldObject = obj;
        heldObject.transform.position = holdPoint.transform.position;
        heldObject.transform.rotation = holdPoint.transform.rotation;
        heldObject.transform.SetParent(holdPoint.transform);
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void PickUp()
    {
        if (swordPrefab != null)
        {
            GameObject sword = Instantiate(swordPrefab, holdPoint.transform.position, Quaternion.identity);
            sword.transform.SetParent(holdPoint.transform);
            sword.GetComponent<Rigidbody>().isKinematic = true;
            heldObject = sword;
        }
    }

    void ReleaseObject()
    {
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        heldObject.transform.parent = null;
        heldObject = null;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Grounded", true); // Персонаж на земле
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Grounded", false); // Персонаж в воздухе
        }
    }
}
