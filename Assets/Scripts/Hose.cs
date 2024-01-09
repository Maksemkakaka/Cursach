using UnityEngine;

public class Hose : MonoBehaviour
{
    public GameObject hydrant; // Гидрант, к которому прикреплен шланг
    public Transform endPoint; // Точка на конце шланга, где расположена система частиц
    public ParticleSystem waterSpray; // Система частиц воды
    public float maxLength = 10f; // Максимальная длина шланга
    public Transform playerHoldingPosition;
    private GameObject[] hoseSegments; // Массив сегментов шланга
    private Vector3 originalScale; // Исходный масштаб сегментов шланга
    private bool isHolding = false; // Переменная, отслеживающая, держит ли игрок шланг

    void Start()
    {
        // Настройте массив сегментов шланга
        hoseSegments = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            hoseSegments[i] = transform.GetChild(i).gameObject;
        }
        originalScale = hoseSegments[0].transform.localScale;
        waterSpray.Stop(); // Убедитесь, что система частиц выключена при старте
    }

    void Update()
    {
        if (isHolding)
        {
            float distanceToHydrant = Vector3.Distance(hydrant.transform.position, playerHoldingPosition.position);
            float segmentLength = distanceToHydrant / (hoseSegments.Length - 1); // Исключаем первый сегмент из расчётов

            // Первый сегмент оставляем без изменений, так как он должен остаться у гидранта
            for (int i = 1; i < hoseSegments.Length; i++)
            {
                GameObject segment = hoseSegments[i];
                Vector3 newScale = segment.transform.localScale;
                newScale.z = segmentLength; // Меняем только z масштаб, чтобы растянуть сегменты
                segment.transform.localScale = newScale;
            }


            if (Input.GetMouseButtonDown(0))
            {
                waterSpray.Play(); // Включаем воду
            }
            else if (Input.GetMouseButtonUp(0))
            {
                waterSpray.Stop(); // Выключаем воду
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
        // Сбросить шланг в исходное состояние
        foreach (var segment in hoseSegments)
        {
            segment.transform.localScale = originalScale;
        }
    }
}
