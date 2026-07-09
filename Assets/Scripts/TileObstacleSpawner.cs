using System.Collections.Generic;
using UnityEngine;

public class TileObstacleSpawner : MonoBehaviour
{
    [Header("Engel Ayarları")]
    public GameObject obstaclePrefab; // engel
    public int maxObstaclesPerTile = 3; // max kac engel

    [Header("Altın Ayarları")]
    public GameObject coinPrefab;
    public int maxCoinsPerTile = 5; // Bir yolda maksimum çıkacak altın sayısı

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        // Yol doğduğunda engel ve altın oluştur
        SpawnObstacles();
        SpawnCoins();
    }

    void SpawnObstacles()
    {
        // 1 ile maxObstaclesPerTile arasında random sayıda engel koy
        int obstaclesToSpawn = Random.Range(1, maxObstaclesPerTile + 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            // Şerit X koordinatları Sol: -3 Orta: 0 Sağ: 3
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];

            // Z ekseninde tileın neresinde dogacak
            float randomZ = Random.Range(-40f, 40f);

            // Engelin doğacağı yer
            Vector3 spawnPosition = new Vector3(randomLaneX, 1f, transform.position.z + randomZ);

            GameObject obs = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

            spawnedObstacles.Add(obs);
        }
    }

    void SpawnCoins()
    {
        // 2 ile maxCoinsPerTile arasında rastgele sayıda altın koy
        int coinsToSpawn = Random.Range(2, maxCoinsPerTile + 1);

        for (int i = 0; i < coinsToSpawn; i++)
        {
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];
            float randomZ = Random.Range(-40f, 40f);

            // Altının doğacağı yer
            Vector3 spawnPosition = new Vector3(randomLaneX, 1f, transform.position.z + randomZ);

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

            
            spawnedObstacles.Add(coin);
        }
    }

    // Yol sahnede yok edildiğinde otomatik çalış
    private void OnDestroy()
    {
        // Yol silinmeden önce kendi listendeki tüm engelleri/altınları sil
        foreach (GameObject obs in spawnedObstacles)
        {
            if (obs != null)
            {
                Destroy(obs);
            }
        }
    }
}