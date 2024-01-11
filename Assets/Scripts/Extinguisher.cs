using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    public ParticleSystem extinguishingParticles; // Система частиц воды или вещества огнетушителя
    public float extinguishDistance = 5f; // Дистанция, на которой огнетушитель может тушить огонь
    public Transform particleEmissionPoint;
    private RaycastHit hit;

    // В предположении, что этот метод является частью скрипта огнетушителя
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Используем particleEmissionPoint для определения начальной точки и направления луча
            Vector3 rayOrigin = particleEmissionPoint.position;
            Vector3 rayDirection = particleEmissionPoint.forward;

            // Визуализируем Raycast для отладки
            Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * extinguishDistance, Color.red);
            int layerMask = 1 << LayerMask.NameToLayer("House"); // Получаем маску слоя для слоя "House"
            layerMask = ~layerMask; // Инвертируем маску, чтобы рейкаст игнорировал слой "House"


            if (Physics.Raycast(rayOrigin, rayDirection, out hit, extinguishDistance, layerMask))
            {
/*                Debug.Log("Raycast hit: " + hit.collider.name); // Выводим имя попавшегося объекта*/

                if (hit.collider.CompareTag("Fire"))
                {
                    FireManager fireManager = hit.collider.GetComponentInParent<FireManager>();
                    if (fireManager != null)
                    { 
                        fireManager.UpdateExtinguishProgress(Time.deltaTime * 10);
                    }
                }
            }
        }
    }






}
