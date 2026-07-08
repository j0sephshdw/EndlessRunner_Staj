using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Yol Ayarları")]
    public GameObject tilePrefab;      // yolcogalt
    public float tileLength = 100f;    // yol uzunluğu y scale
    public int numberOfTiles = 3;      // erkanda aynı anda 3 yol olsn şimdlk
    public Transform playerTransform;  // Karakter konumu

    private float spawnZ = 0f;         // yeni yolun dogacagı z koordnat
    private List<GameObject> activeTiles = new List<GameObject>(); // ahnedekı aktıf yollar listes

    void Start()
    {
        // Oyun başlarken ilk 3 yol parçasını peş peşe oluştur
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        /* Karakter bulunduğu yolun yarısını geçtiğinde Z - 50, 
         önüne yeni yol koy ve en arkada kalanı sil.*/
        if (playerTransform.position.z - 80 > (spawnZ - numberOfTiles * tileLength))
        {
            SpawnTile();
            DeleteTile();
        }
    }

    // Yeni yol oluştur fonkKsyonu
    void SpawnTile()
    {
        GameObject go = Instantiate(tilePrefab, Vector3.forward * spawnZ, Quaternion.identity);
        activeTiles.Add(go);       // Yeni yolu listeye ekle
        spawnZ += tileLength;      // Bir sonraki yol için doğma noktasını ileri taşı
    }

    // Arkadaki yolu sil fonk
    void DeleteTile()
    {
        Destroy(activeTiles[0]);   // Listedeki en eski (0. index) yolu sil
        activeTiles.RemoveAt(0);   // Listeden de çıkar
    }
}