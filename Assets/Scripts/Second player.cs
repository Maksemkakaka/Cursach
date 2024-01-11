using UnityEngine;

public class SecondPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f; // �������� ��������
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    public Camera playerCamera;
    public LayerMask enemyLayer;
    public GameObject holdPoint; // ����� ������� �� ������
    private GameObject heldObject; // ������ �� ������, ������� ������ � ������ ������
    private Animator animator; 
    public GameObject swordPrefab;
    public float attackSphereRadius = 2f; // ������ ����� �����


    // ���������� ��� ���������� ��������� ���������
    private float pitch = 0f; // ���� ������� �����-����
    public float pitchSpeed = 100f; // �������� ������� ������
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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
        // ����� ���� � ����������� �������
        if (heldObject != null)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject.transform.parent = null;
            Destroy(heldObject); // ���������� ������ ����
            heldObject = null;
        }
    }
    public void CheckHit()
    {
        // ������� ��� ����� ����� - ����� �������
        Vector3 spherePosition = playerCamera.transform.position + playerCamera.transform.forward * attackSphereRadius;

        // ������������� ������� �����
        DrawDebugSphere(spherePosition, attackSphereRadius, Color.red, 0.5f);

        // �������� ���� ������ � ����� �����
        Collider[] hitEnemies = Physics.OverlapSphere(spherePosition, attackSphereRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // ���������, ������������� �� ������ - ����
            if (enemy.CompareTag("Enemy"))
            {
                // ������� ���� �����
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
        // �������� ���� � ������ ���������
        float moveHorizontal = Input.GetAxis("Horizontal2");
        float moveVertical = Input.GetAxis("Vertical2");

        // ����������� ����������� ����� �� ��������� ��������� � �������, ��������� ����������� ������� ������
        Vector3 movement = playerCamera.transform.right * moveHorizontal + playerCamera.transform.forward * moveVertical;
        movement.y = 0; // ���������� ������������ ������������, ����� �������� �� ����� � �������

        // ��������� ��������
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);

        // ������������� ��������� ��������
        animator.SetBool("isRunning", movement.magnitude > 0);
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        animator.SetTrigger("Jump"); // ���������� �������� ������
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
            // �������� ����� ��������� ������ ��������
            if (hit.collider.CompareTag("Ladder"))
            {
                GrabObject(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Chest"))
            {
                // �������������� � ������ ��� �������� ����
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
            animator.SetBool("Grounded", true); // �������� �� �����
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("Grounded", false); // �������� � �������
        }
    }
}
