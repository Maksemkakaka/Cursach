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
        extinguishProgress += amount;
        if (extinguishProgress >= extinguishThreshold)
        {
            ExtinguishFire();
            extinguishProgress = extinguishThreshold;
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
            Destroy(currentFire);
            currentFire = null;
            extinguishProgress = 0f;
            isFireActive = false;
        }
    }
}
