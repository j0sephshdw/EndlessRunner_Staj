using System.Collections.Generic;
using UnityEngine;

public class TileObstacleSpawner : MonoBehaviour
{
    [Header("Engel Ayarları")]
    public GameObject obstaclePrefab; // engel
    public GameObject highObstaclePrefab;//yüksek engel
    public int maxObstaclesPerTile = 3; // max kac engel

    [Header("Altın Ayarları")]
    public GameObject coinPrefab;
    public int maxCoinsPerTile = 5; // Bir yolda maksimum çıkacak altın sayısı

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        if (transform.position.z < 50f)//yolun cok basndaysa engel koyma
        {
            return;
        }
        // Yol doğduğunda engel ve altın oluştur
        SpawnObstacles();
        SpawnCoins();
    }

    void SpawnObstacles()
    {
        int obstaclesToSpawn = Random.Range(1, maxObstaclesPerTile + 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];

            float randomZ = 0;
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;

            // Güvenli bir yer bulana kadar maksimum 10 kez rastgele sayı dene
            while (!validPosition && attempts < 10)
            {
                randomZ = Random.Range(-40f, 40f);
                spawnPosition = new Vector3(randomLaneX, 1f, transform.position.z + randomZ);
                validPosition = true;

                // Mevcut üretilmiş her şeyi kontrol et (Önceki engeller veya paralar)
                foreach (GameObject existingObj in spawnedObstacles)
                {
                    if (existingObj != null && Mathf.Abs(existingObj.transform.position.x - randomLaneX) < 0.5f)
                    {
                        // Aynı şeritteyseler, aralarında en az 15 birim mesafe olmalı (Dip dibe engel olmasın)
                        if (Mathf.Abs(existingObj.transform.position.z - spawnPosition.z) < 15f)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
                attempts++;
            }

            if (!validPosition) continue; // Güvenli yer bulunamadıysa bu engeli üretmeyi pas geç

            GameObject selectedObstacle;
            float spawnY;

            if (Random.value > 0.5f)
            {
                selectedObstacle = obstaclePrefab;
                spawnY = 1f;
            }
            else
            {
                selectedObstacle = highObstaclePrefab;
                spawnY = 2.6f;
            }

            spawnPosition.y = spawnY;

            GameObject obs = Instantiate(selectedObstacle, spawnPosition, Quaternion.identity);
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

                // Paranın bir engelin içine veya çok yakınına doğmasını engelle
                foreach (GameObject existingObj in spawnedObstacles)
                    if (existingObj != null && Mathf.Abs(existingObj.transform.position.x - randomLaneX) < 0.5f)
                    {
                        // Bir engelin veya başka bir paranın en az 6 birim uzağında olmalı
                        if (Mathf.Abs(existingObj.transform.position.z - spawnPosition.z) < 6f)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                attempts++;
            }

            if (!validPosition) continue;

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