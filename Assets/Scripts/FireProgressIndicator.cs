using UnityEngine;
using UnityEngine.UI;

public class FireProgressIndicator : MonoBehaviour
{
    public Image progressIndicator; // ������� UI ��� ����������
    public FireManager fireManager; // ������ �� FireManager
    private FireExtinguishing targetFireExtinguishing;

    public void Initialize(FireExtinguishing fireExtinguishing)
    {
        fireManager = fireExtinguishing.GetComponentInParent<FireManager>();
        targetFireExtinguishing = fireExtinguishing;
    }


    private void Update()
    {

        if (targetFireExtinguishing != null && targetFireExtinguishing.IsFireActive)
        {
            // ��������� ��������� ��������� �� ������ ��������� ������� ����
            float progress = fireManager.extinguishProgress / fireManager.extinguishThreshold;
            progressIndicator.fillAmount = progress;
            progressIndicator.gameObject.SetActive(true);


        }
        else
        {
            progressIndicator.gameObject.SetActive(false); // �������� ���������, ����� ����� �������
        }
    }

    public void SetFireManager(FireManager manager)
    {
        fireManager = manager;
    }
}
