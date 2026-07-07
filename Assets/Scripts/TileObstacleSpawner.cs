using UnityEngine;

public class TileObstacleSpawner : MonoBehaviour
{
    [Header("Engel Ayarları")]
    public GameObject obstaclePrefab; // engel
    public int maxObstaclesPerTile = 3; // max kac engel

    void Start()
    {
        // Yol doğduğunda engel olustr
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        // 1 ile maxObstaclesPerTile arasında random sayıda engel koy
        int obstaclesToSpawn = Random.Range(1, maxObstaclesPerTile + 1);

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            // Şerit X koordinatlarıSol: -3 Orta: 0 Sağ: 3
            float[] lanes = { -3f, 0f, 3f };
            float randomLaneX = lanes[Random.Range(0, lanes.Length)];

            // Z ekseninde  tileın neresinde dogacak
            float randomZ = Random.Range(-40f, 40f);

            // Engelin doğacağıyer
            Vector3 spawnPosition = new Vector3(randomLaneX, 1f, transform.position.z + randomZ);

            
            GameObject obs = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

            // engeli 15 saniye sonra sahneden sil (Memory Leak olmasınn)
            Destroy(obs, 15f);
        }
    }
}