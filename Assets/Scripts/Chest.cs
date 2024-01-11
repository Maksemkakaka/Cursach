using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject swordPrefab; // Префаб меча, который будет выдавать ящик

    public GameObject GiveSword()
    {
        if (swordPrefab != null)
        {
            // Создаем меч рядом с ящиком или в специально отведенном месте
            GameObject sword = Instantiate(swordPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            return sword;
        }
        return null;
    }
}
