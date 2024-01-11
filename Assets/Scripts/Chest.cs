using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject swordPrefab; // ������ ����, ������� ����� �������� ����

    public GameObject GiveSword()
    {
        if (swordPrefab != null)
        {
            // ������� ��� ����� � ������ ��� � ���������� ���������� �����
            GameObject sword = Instantiate(swordPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            return sword;
        }
        return null;
    }
}
