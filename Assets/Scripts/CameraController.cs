using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public Transform target; // Takip et ninja
    public float smoothSpeed = 10f; // Kamera yumuşaklık

    private Vector3 offset; // Oyun başı kamera mesafesi

    void Start()
    {
        // Oyun başladığnda kamera ile karakter arasındaki mesafeyi hafızayaa al
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }
    void LateUpdate()
    {
        if (target == null)
            return;

        // X ve Y  eksenleri yumusatlms geçiş
        float newX = Mathf.Lerp(transform.position.x, target.position.x + offset.x, smoothSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, target.position.y + offset.y, smoothSpeed * Time.deltaTime);

        // Z ekseni gecikmesiz takip
        float newZ = target.position.z + offset.z;

        // Kamera pozisyon güncelle
        transform.position = new Vector3(newX, newY, newZ);
    }
}