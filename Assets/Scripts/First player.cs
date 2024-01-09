using UnityEngine;

public class FirstPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f; // �������� ��������
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f; // ���������������� ����
    private Rigidbody rb;
    private bool isGrounded;
    public GameObject holdPoint; // ����� ������� �� ������
    private GameObject heldObject; // ������ �� ������, ������� ������ � ������ ������
    public Camera playerCamera;
    private bool isClimbing = false;
    private float xRotation = 0f; // ��� ���������� �������� ������ �� ��� X
    public float climbSpeed = 5f;
    private Hose hoseComponent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // ������������� ������ � ������ ������
        hoseComponent = FindObjectOfType<Hose>();
    }

    void Update()
    {
        // ����������� �����/������
        float moveHorizontal = Input.GetAxis("Horizontal");
        // ����������� �����/�����
        float moveVertical = Input.GetAxis("Vertical");
        // ��������� ��������
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);

        // �������� ��������� � ������� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY; // ��������� ������� �� ���������
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ������������ �������� �� ��� X

        // ��������� �������� � ������ � ���������
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (Input.GetMouseButtonDown(0) && heldObject != null && heldObject.CompareTag("FireExtinguisher"))
        {
            // ���������� �������-�������
            ToggleExtinguisherParticle(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // ������������ �������-�������
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
        // ���� � ����� ��� ���� ������, �� ��� ���������
        if (heldObject != null)
        {
            ReleaseObject();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // �������� ��� ������ ��� �����������

            if (hit.collider.CompareTag("FireExtinguisher") || hit.collider.CompareTag("Ladder"))
            {
                GrabObject(hit.collider.gameObject);
            }
        }
    }


    void GrabObject(GameObject obj)
    {
        heldObject = obj;
        // ������������� ������ ����� �� ������� ����� �������
        heldObject.transform.position = holdPoint.transform.position;
        // ������������ ������ ���, ����� �� ������� � ��� �� �����������, ��� � ����� �������
        // ����� ����� ���������� �������� �������, ��������, (0,0,0) ��� ������������ ������������
        heldObject.transform.rotation = holdPoint.transform.rotation;
        // ������ ������ �������� ��������� ����� �������, ����� �� �������� �� ���������� ������
        heldObject.transform.parent = holdPoint.transform;
        // ��������� ������ �������, ����� �� �� �����, ���� �� ��� ������
        heldObject.GetComponent<Rigidbody>().isKinematic = true;
    }


    void ReleaseObject()
    {
        // ���������� ������ �������
        heldObject.GetComponent<Rigidbody>().isKinematic = false;
        // ������� ������������ �����
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
            rb.isKinematic = true; // ��������� ������, ����� ������������� �������
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
