using UnityEngine;

public class SecondPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f; // �������� ��������
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    public Camera playerCamera;

    public GameObject holdPoint; // ����� ������� �� ������
    private GameObject heldObject; // ������ �� ������, ������� ������ � ������ ������


    // ���������� ��� ���������� ��������� ���������
    private float pitch = 0f; // ���� ������� �����-����
    public float pitchSpeed = 100f; // �������� ������� ������
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();

        // �������� ���� � ������� ����� ��� �������� ���������
        float turnAmount = Input.GetAxis("RightHorizontal") * turnSpeed * Time.deltaTime;
        // �������� ���� � ������� ����� ��� ������� ������ �����-����
        float pitchAmount = Input.GetAxis("RightVertical") * pitchSpeed * Time.deltaTime;

        // ������������ ���������
        transform.Rotate(0, turnAmount, 0);

        // ���������� ���� ������� ������ � ��������� �����������, ����� ������ �� ���������������� ���������
        pitch = Mathf.Clamp(pitch - pitchAmount, -45f, 45f);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, playerCamera.transform.localEulerAngles.y, 0);

        if (Input.GetKeyDown(KeyCode.JoystickButton3) && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1")) // ��������������, ��� Fire1 ��������� �� ������ ��� ��������������
        {
            Interact();
        }
    }

    void Move()
    {
        // �������� ���� � ������ ���������
        float moveHorizontal = Input.GetAxis("Horizontal2");
        float moveVertical = Input.GetAxis("Vertical2");

        // ����������� ����������� ����� �� ��������� ��������� � �������, ��������� ����������� ������� ������
        Vector3 movement = playerCamera.transform.right * moveHorizontal + playerCamera.transform.forward * moveVertical;
        movement.y = 0; // ���������� ������������ ������������, ����� �������� �� ����� � �������

        // ��������� ��������
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void Interact()
    {
        // ���� � ����� ��� ���� ������, �� ��� ���������
        if (heldObject != null)
        {
            ReleaseObject();
            return;
        }

        // ���������, ���� �� ����� ���� ������ ��� ��������������
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            // ���������, �������� �� ������ �������������
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
