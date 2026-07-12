using System.Collections.Generic;
using UnityEngine;

public class TileObstacleSpawner : MonoBehaviour
{
    [Header("Engel Ayarları")]
    public GameObject obstaclePrefab;     // Normal engel
    public GameObject highObstaclePrefab; // Yüksek engel 
    public GameObject busBlockPrefab;     // Geçilemez Düz Otobüs
    public GameObject busRampPrefab;      // Üstüne Çıkılabilen Rampalı Otobüs

    [Tooltip("Oyunun en başında (0. metrelerde) çıkabilecek maksimum engel sayısı")]
    public int baseMaxObstacles = 2;
    [Tooltip("Oyunun ilerleyen safhalarında çıkabilecek en fazla engel limiti (Örn: En fazla 5)")]
    public int absoluteMaxObstacles = 5;

    [Header("Altın Ayarları")]
    public GameObject coinPrefab;
    public int maxCoinsPerTile = 5;

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        if (transform.position.z < 50f) // Yolun çok başındaysa engel koyma
        {
            return;
        }

        // Yol doğduğunda engel ve altın oluştur
        SpawnObstacles();
        SpawnCoins();
    }

    void SpawnObstacles()
    {
        // --- DİNAMİK ZORLUK AYARI ---
        // Oyuncu her 200 metrede bir fazladan 1 engel potansiyeline sahip olsun
        // Formül: Başlangıç engeli + (Mevcut Z Pozisyonu / 200)
        int calculatedMaxObstacles = baseMaxObstacles + Mathf.FloorToInt(transform.position.z / 200f);

        // Hesaplanan değerin belirli tavan sınırı aşma
        int currentMaxObstacles = Mathf.Clamp(calculatedMaxObstacles, baseMaxObstacles, absoluteMaxObstacles);

        // Bu yol parçası için 1 ile max engel sayısı arasıb da randomsec
        int obstaclesToSpawn = Random.Range(1, currentMaxObstacles + 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];

            float randomZ = 0;
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition && attempts < 10)
            {
                randomZ = Random.Range(-40f, 40f);
                spawnPosition = new Vector3(randomLaneX, 0f, transform.position.z + randomZ);
                validPosition = true;

                foreach (GameObject existingObj in spawnedObstacles)
                {
                    if (existingObj != null && Mathf.Abs(existingObj.transform.position.x - randomLaneX) < 0.5f)
                    {
                        // Aynı şeritteyseler, aralarında en az 15 birim mesafe olmalı
                        if (Mathf.Abs(existingObj.transform.position.z - spawnPosition.z) < 15f)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
                attempts++;
            }

            if (!validPosition) continue;

            GameObject selectedObstacle = null;
            float spawnY = 0f;

            int randomObstacleType = Random.Range(0, 4);

            if (randomObstacleType == 0)
            {
                selectedObstacle = obstaclePrefab;
                spawnY = 1f;
            }
            else if (randomObstacleType == 1)
            {
                selectedObstacle = highObstaclePrefab;
                spawnY = 2.6f;
            }
            else if (randomObstacleType == 2)
            {
                selectedObstacle = busBlockPrefab;
                spawnY = 0f;
            }
            else if (randomObstacleType == 3)
            {
                selectedObstacle = busRampPrefab;
                spawnY = 0f;
            }

            spawnPosition.y = spawnY;

            GameObject obs = Instantiate(selectedObstacle, spawnPosition, selectedObstacle.transform.rotation);
            spawnedObstacles.Add(obs);
        }
    }

    void SpawnCoins()
    {
        int coinsToSpawn = Random.Range(2, maxCoinsPerTile + 1);

        for (int i = 0; i < coinsToSpawn; i++)
        {
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];

            float randomZ = 0;
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition && attempts < 10)
            {
                randomZ = Random.Range(-40f, 40f);
                spawnPosition = new Vector3(randomLaneX, 1f, transform.position.z + randomZ);
                validPosition = true;

                foreach (GameObject existingObj in spawnedObstacles)
                {
                    if (existingObj != null && Mathf.Abs(existingObj.transform.position.x - randomLaneX) < 0.5f)
                    {
                        if (Mathf.Abs(existingObj.transform.position.z - spawnPosition.z) < 6f)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
                attempts++;
            }

            if (!validPosition) continue;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            spawnedObstacles.Add(coin);
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject obs in spawnedObstacles)
        {
            if (obs != null)
            {
                Destroy(obs);
            }
        }
    }
}