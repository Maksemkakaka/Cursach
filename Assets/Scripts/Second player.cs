using UnityEngine;

public class SecondPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f; // Скорость поворота
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    public Camera playerCamera;

    public GameObject holdPoint; // Точка захвата на камере
    private GameObject heldObject; // Ссылка на объект, который держим в данный момент


    // Переменные для управления вращением персонажа
    private float pitch = 0f; // Угол наклона вверх-вниз
    public float pitchSpeed = 100f; // Скорость наклона камеры
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

        // Регулируем угол наклона камеры и применяем ограничение, чтобы камера не переворачивалась полностью
        pitch = Mathf.Clamp(pitch - pitchAmount, -45f, 45f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, playerCamera.transform.localEulerAngles.y, 0);

        if (Input.GetKeyDown(KeyCode.JoystickButton3) && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1")) // Предполагается, что Fire1 назначена на кнопку для взаимодействия
        {
            Interact();
        }
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

        // Проверяем, есть ли перед нами объект для взаимодействия
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            // Проверяем, является ли объект огнетушителем
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


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
