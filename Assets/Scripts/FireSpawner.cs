using UnityEngine;
using System.Collections.Generic;

public class FireSpawner : MonoBehaviour
{
    public List<Transform> fireSpawnPoints; // Список точек спауна огня
    public GameObject firePrefab; // Префаб огня
    public float spawnInterval = 10f; // Интервал между спаунами огня
    public GameObject indicatorPrefab;
    public Transform canvasTransform;

    private float timeSinceLastSpawn;

    void Start()
    {
        timeSinceLastSpawn = spawnInterval;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnFire();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnFire()
    {
        if (fireSpawnPoints.Count == 0) return;

        int spawnIndex = Random.Range(0, fireSpawnPoints.Count);
        Transform spawnPoint = fireSpawnPoints[spawnIndex];

        if (spawnPoint.childCount == 0)
        {
            GameObject newFire = Instantiate(firePrefab, spawnPoint.position, Quaternion.identity);
            newFire.transform.SetParent(spawnPoint, true);

            FireManager fireManager = newFire.GetComponent<FireManager>();
            fireManager.currentFire = newFire;

            // Создайте индикатор прогресса для нового огня
            GameObject newIndicator = Instantiate(indicatorPrefab, canvasTransform);
            newIndicator.GetComponent<FireProgressIndicator>().SetFireManager(fireManager);
        }
    }


}
