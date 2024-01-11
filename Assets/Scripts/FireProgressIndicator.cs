using UnityEngine;
using UnityEngine.UI;

public class FireProgressIndicator : MonoBehaviour
{
    public Image progressIndicator; // Ёлемент UI дл€ индикатора
    public FireManager fireManager; // —сылка на FireManager
    private FireExtinguishing targetFireExtinguishing;

    public void Initialize(FireExtinguishing fireExtinguishing)
    {
        fireManager = fireExtinguishing.GetComponentInParent<FireManager>();
        targetFireExtinguishing = fireExtinguishing;
        Debug.Log($"FireExtinguishing assigned: {targetFireExtinguishing != null}, Active: {targetFireExtinguishing?.IsFireActive}");
    }


    private void Update()
    {

        if (targetFireExtinguishing != null && targetFireExtinguishing.IsFireActive)
        {
            // ќбновл€ем индикатор прогресса на основе прогресса тушени€ огн€
            float progress = fireManager.extinguishProgress / fireManager.extinguishThreshold;
            progressIndicator.fillAmount = progress;
            progressIndicator.gameObject.SetActive(true);


        }
        else
        {
            progressIndicator.gameObject.SetActive(false); // —крываем индикатор, когда огонь потушен
        }
    }

    public void SetFireManager(FireManager manager)
    {
        fireManager = manager;
    }
}
