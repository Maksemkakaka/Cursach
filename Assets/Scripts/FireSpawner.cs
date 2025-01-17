using UnityEngine;
using System.Collections.Generic;

public class FireSpawner : MonoBehaviour
{
    public List<Transform> fireSpawnPoints; // ������ ����� ������ ����
    public GameObject firePrefab; // ������ ����
    public float spawnInterval = 10f; // �������� ����� �������� ����
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
            if (fireManager != null)
            {
                fireManager.currentFire = newFire;
                fireManager.isFireActive = true; 
            }


            GameObject newIndicator = Instantiate(indicatorPrefab, canvasTransform);
            FireProgressIndicator progressIndicator = newIndicator.GetComponent<FireProgressIndicator>();
            if (progressIndicator != null)
            {
                progressIndicator.SetFireManager(fireManager);
                progressIndicator.Initialize(newFire.GetComponent<FireExtinguishing>()); 
            }

        }
    }


}
