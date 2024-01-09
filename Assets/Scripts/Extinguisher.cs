using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    public ParticleSystem extinguishingParticles; // ������� ������ ���� ��� �������� ������������
    public float extinguishDistance = 5f; // ���������, �� ������� ������������ ����� ������ �����
    public Transform particleEmissionPoint;
    private RaycastHit hit;

    // � �������������, ��� ���� ����� �������� ������ ������� ������������
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // ���������� particleEmissionPoint ��� ����������� ��������� ����� � ����������� ����
            Vector3 rayOrigin = particleEmissionPoint.position;
            Vector3 rayDirection = particleEmissionPoint.forward;

            // ������������� Raycast ��� �������
            Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * extinguishDistance, Color.red);

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, extinguishDistance))
            {
                Debug.Log("Raycast hit: " + hit.collider.name); // ������� ��� ����������� �������

                if (hit.collider.CompareTag("Fire"))
                {
                    FireManager fireManager = hit.collider.GetComponentInParent<FireManager>();
                    if (fireManager != null)
                    {
                        Debug.Log("Extinguishing fire");
                        fireManager.UpdateExtinguishProgress(Time.deltaTime * 10);
                    }
                }
            }
        }
    }






}
