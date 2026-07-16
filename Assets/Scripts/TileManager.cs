using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Yol Ayarları")]
    public GameObject tilePrefab;      // oluşturulacak yol objesi
    public float tileLength = 100f;    // Yolun z eksenindeki uzunluğu
    public int numberOfTiles = 3;      // Sahnede aynı anda bulunacak maksimum yol sayısı
    public Transform playerTransform;  // Karakter konumu

    private float spawnZ = 0f;         // Yeni yolun ekleneceği Z pozisyonu
    private List<GameObject> activeTiles = new List<GameObject>(); // Sahnede aktif olan yolları tuttuğumuz liste

    void Start()
    {
        // Oyun başlarken ilk 3 yol parçasını peş peşe oluşturuyoruz
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        // Karakter ilerledikçe arkada kalan yolları silip ileriye yenisini ekliyoruz
        if (playerTransform.position.z - 80 > (spawnZ - numberOfTiles * tileLength))
        {
            SpawnTile();
            DeleteTile();
        }
    }

    void SpawnTile()
    {
        GameObject go = Instantiate(tilePrefab, Vector3.forward * spawnZ, Quaternion.identity);
        activeTiles.Add(go);       // Yeni yolu listeye ekle
        spawnZ += tileLength;      // Bir sonraki yol için Zyi ileri taşı
    }

    void DeleteTile()
    {
        Destroy(activeTiles[0]);   // Listedeki en eski yolu yok et (performans için )
        activeTiles.RemoveAt(0);
    }
}