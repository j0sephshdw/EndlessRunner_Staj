using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public Transform target; // Takip edilecek Player
    public float smoothSpeed = 10f; // Kameranın takip hızı yumuşak geçiş

    private Vector3 offset; // Oyun başındaki orijinal kamera mesafesi

    void Start()
    {
        // Oyun başladığında kamera ile karakter arasındaki başlangıç mesafesini kaydet
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // X ve Y eksenlerinde sarsıntısız geçiş
        float newX = Mathf.Lerp(transform.position.x, target.position.x + offset.x, smoothSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, target.position.y + offset.y, smoothSpeed * Time.deltaTime);

        // Z ekseninde  gecikmesiz tam takip
        float newZ = target.position.z + offset.z;

        // Yeni pozisyonu uygula
        transform.position = new Vector3(newX, newY, newZ);
    }
}