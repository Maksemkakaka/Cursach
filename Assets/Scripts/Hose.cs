using UnityEngine;

public class Hose : MonoBehaviour
{
    public GameObject hydrant; // �������, � �������� ���������� �����
    public Transform endPoint; // ����� �� ����� ������, ��� ����������� ������� ������
    public ParticleSystem waterSpray; // ������� ������ ����
    public float maxLength = 10f; // ������������ ����� ������
    public Transform playerHoldingPosition;
    private GameObject[] hoseSegments; // ������ ��������� ������
    private Vector3 originalScale; // �������� ������� ��������� ������
    private bool isHolding = false; // ����������, �������������, ������ �� ����� �����

    void Start()
    {
        // ��������� ������ ��������� ������
        hoseSegments = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            hoseSegments[i] = transform.GetChild(i).gameObject;
        }
        originalScale = hoseSegments[0].transform.localScale;
        waterSpray.Stop(); // ���������, ��� ������� ������ ��������� ��� ������
    }

    void Update()
    {
        if (isHolding)
        {
            float distanceToHydrant = Vector3.Distance(hydrant.transform.position, playerHoldingPosition.position);
            float segmentLength = distanceToHydrant / (hoseSegments.Length - 1); // ��������� ������ ������� �� ��������

            // ������ ������� ��������� ��� ���������, ��� ��� �� ������ �������� � ��������
            for (int i = 1; i < hoseSegments.Length; i++)
            {
                GameObject segment = hoseSegments[i];
                Vector3 newScale = segment.transform.localScale;
                newScale.z = segmentLength; // ������ ������ z �������, ����� ��������� ��������
                segment.transform.localScale = newScale;
            }


            if (Input.GetMouseButtonDown(0))
            {
                waterSpray.Play(); // �������� ����
            }
            else if (Input.GetMouseButtonUp(0))
            {
                waterSpray.Stop(); // ��������� ����
            }
        }
    }

    public void GrabHose()
    {
        isHolding = true;
    }

    public void ReleaseHose()
    {
        isHolding = false;
        waterSpray.Stop();
        // �������� ����� � �������� ���������
        foreach (var segment in hoseSegments)
        {
            segment.transform.localScale = originalScale;
        }
    }
}
