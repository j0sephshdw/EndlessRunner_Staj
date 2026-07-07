using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public Transform target; // oyuncu objesn takp et
    private Vector3 offset;  // Kamera ile karakter arasındaki başlangıç mesafesi

    void Start()
    {
        // baslangıcta kameranın o anki konumu ile karakterin konumu farkını kaydet
        offset = transform.position - target.position;
    }

    // Kamera takip lateupdate
    void LateUpdate()
    {
        // Eğer karakter yoksa veya oyun bittiyse hata vermemesi için
        if (target == null) return;

        // X sagsol ve Y yukarıasagı sabit tut, sadece Z ileri ekseninde takip et
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, target.position.z + offset.z);

        // Kameranın pozisyonu= yeni pozisyon
        transform.position = newPosition;
    }
}