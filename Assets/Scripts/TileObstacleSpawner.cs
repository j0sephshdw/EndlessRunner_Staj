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
    [Tooltip("Oyunun ilerleyen safhalarında çıkabilecek en fazla engel limiti")]
    public int absoluteMaxObstacles = 5;

    [Header("Altın Ayarları")]
    public GameObject coinPrefab;
    public int maxCoinsPerTile = 5;

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        // Yolun çok başındaysa oyuncuya tepki süresi vermek için engel koyma
        if (transform.position.z < 50f)
        {
            return;
        }

        SpawnObstacles();
        SpawnCoins();
    }

    void SpawnObstacles()
    {
        // İlerledikçe zorluğun artması için dinamik engel sayısı hesaplama
        int calculatedMaxObstacles = baseMaxObstacles + Mathf.FloorToInt(transform.position.z / 200f);
        int currentMaxObstacles = Mathf.Clamp(calculatedMaxObstacles, baseMaxObstacles, absoluteMaxObstacles);
        int obstaclesToSpawn = Random.Range(1, currentMaxObstacles + 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];

            float randomZ = 0;
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;
            int obstacleType = -1;

            // Objelerin üst üste binmemesi için uygun pozisyon ara (Max 20 deneme)
            while (!validPosition && attempts < 20)
            {
                randomZ = Random.Range(-40f, 40f);
                spawnPosition = new Vector3(randomLaneX, 0f, transform.position.z + randomZ);

                // 0: Normal, 1: Yüksek, 2: Düz Otobüs, 3: Rampalı Otobüs
                obstacleType = Random.Range(0, 4);
                validPosition = true;

                // Aynı şeritteki engellerin birbirine yakınlığını kontrol et
                foreach (GameObject existingObj in spawnedObstacles)
                {
                    if (existingObj != null && Mathf.Abs(existingObj.transform.position.x - randomLaneX) < 0.5f)
                    {
                        // Engeller arası en az 15 birim mesafe olmalı
                        if (Mathf.Abs(existingObj.transform.position.z - spawnPosition.z) < 15f)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }

                if (!validPosition)
                {
                    attempts++;
                    continue;
                }

                // 3 şerit birden kapanıp oyuncuyu haksız yere öldürmesin diye güvenlik kontrolü
                if (obstacleType == 2)
                {
                    bool leftFlatBlocked = false;
                    bool centerFlatBlocked = false;
                    bool rightFlatBlocked = false;

                    foreach (GameObject existingObj in spawnedObstacles)
                    {
                        if (existingObj != null && Mathf.Abs(existingObj.transform.position.z - spawnPosition.z) < 12f)
                        {
                            if (existingObj.name.Contains(busBlockPrefab.name))
                            {
                                if (existingObj.transform.position.x < -1.5f) leftFlatBlocked = true;
                                else if (existingObj.transform.position.x > 1.5f) rightFlatBlocked = true;
                                else centerFlatBlocked = true;
                            }
                        }
                    }

                    // Şu an spawn etmeye çalıştığımız şeridi de kapalı var say
                    if (randomLaneX < -1.5f) leftFlatBlocked = true;
                    else if (randomLaneX > 1.5f) rightFlatBlocked = true;
                    else centerFlatBlocked = true;

                    // Eğer 3 şerit de düz otobüs ile kapandıysa bu pozisyonu iptal et
                    if (leftFlatBlocked && centerFlatBlocked && rightFlatBlocked)
                    {
                        validPosition = false;
                    }
                }
                attempts++;
            }

            if (!validPosition) continue;

            GameObject selectedObstacle = null;
            float spawnY = 0f;

            if (obstacleType == 0)
            {
                selectedObstacle = obstaclePrefab;
                spawnY = 1f;
            }
            else if (obstacleType == 1)
            {
                selectedObstacle = highObstaclePrefab;
                spawnY = 2.6f;
            }
            else if (obstacleType == 2)
            {
                selectedObstacle = busBlockPrefab;
                spawnY = 0f;
            }
            else if (obstacleType == 3)
            {
                selectedObstacle = busRampPrefab;
                spawnY = 0f;
            }

            spawnPosition.y = spawnY;
            GameObject obs = Instantiate(selectedObstacle, spawnPosition, selectedObstacle.transform.rotation);
            obs.name = selectedObstacle.name;
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

                // Altınların engellerin tam içine doğmasını engelle
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
        // Performans optimizasyonu: Tile yok olduğunda üstündeki her şeyi temizle
        foreach (GameObject obs in spawnedObstacles)
        {
            if (obs != null)
            {
                Destroy(obs);
            }
        }
    }
}