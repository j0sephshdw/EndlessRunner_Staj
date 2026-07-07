using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float forwardSpeed = 10f;     // İleri doğru koşma hızı
    public float laneDistance = 3f;      // Şeritler arası mesafe
    public float laneChangeSpeed = 10f;  // Şerit değiştirme yumuşaklığı (Lerp hızı)

    private int desiredLane = 1;         // 0: Sol, 1: Orta, 2: Sağ Oyuna ortadan başla

    void Update()
    {
        // ileri hareket
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;

        // Sağ ve Sol girdidf
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2; // En sağdaysa daha sağa gitmesin
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0; // En soldaysa sola gitmesin
        }

        // Hedef şeridin X pozisyonunu hesapla
        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 1) targetX = 0f;
        else if (desiredLane == 2) targetX = laneDistance;

        // Pürüzsüz geçiş Vector3.Lerp kullan
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneChangeSpeed * Time.deltaTime);
    }
}