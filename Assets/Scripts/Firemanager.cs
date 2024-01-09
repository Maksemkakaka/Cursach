using UnityEngine;

public class FireManager : MonoBehaviour
{
    public GameObject firePrefab; // Префаб огня с системой частиц
    public float extinguishProgress = 0f; // Прогресс тушения огня
    public float extinguishThreshold = 100f;
    public bool isFireActive = true;
    public GameObject currentFire;

    public void UpdateExtinguishProgress(float amount)
    {
        if (currentFire == null)
        {
            Debug.LogError("Ошибка: currentFire не установлен.");
            return;
        }

        extinguishProgress += amount;
        if (extinguishProgress >= extinguishThreshold)
        {
            ExtinguishFire();
            // Установите прогресс в максимальное значение, чтобы индикатор был заполнен полностью
            extinguishProgress = extinguishThreshold;
            // Теперь вызовите метод для завершения тушения
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
            Debug.Log("Огонь потушен!"); // Выводим сообщение о том, что огонь потушен
            Destroy(currentFire);
            currentFire = null;
            extinguishProgress = 0f; // Сбрасываем прогресс тушения огня
            isFireActive = false; // Огонь теперь не активен
        }
    }
}
