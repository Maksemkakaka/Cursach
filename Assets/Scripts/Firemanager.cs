using UnityEngine;

public class FireManager : MonoBehaviour
{
    public GameObject firePrefab; // ������ ���� � �������� ������
    public float extinguishProgress = 0f; // �������� ������� ����
    public float extinguishThreshold = 100f;
    public bool isFireActive = true;
    public GameObject currentFire;

    public void UpdateExtinguishProgress(float amount)
    {
        if (currentFire == null)
        {
            Debug.LogError("������: currentFire �� ����������.");
            return;
        }

        extinguishProgress += amount;
        if (extinguishProgress >= extinguishThreshold)
        {
            ExtinguishFire();
            // ���������� �������� � ������������ ��������, ����� ��������� ��� �������� ���������
            extinguishProgress = extinguishThreshold;
            // ������ �������� ����� ��� ���������� �������
            FireExtinguishing fireExtinguishing = currentFire.GetComponent<FireExtinguishing>();
            if (fireExtinguishing != null)
            {
                fireExtinguishing.CompleteExtinguishing();
            }
        }
    }



    public void ExtinguishFire()
    {
        if (currentFire != null)
        {
            Debug.Log("����� �������!"); // ������� ��������� � ���, ��� ����� �������
            Destroy(currentFire);
            currentFire = null;
            extinguishProgress = 0f; // ���������� �������� ������� ����
            isFireActive = false; // ����� ������ �� �������
        }
    }
}
