using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    [Header("Hareket Ayarları")]
    public float forwardSpeed = 10f;     // İleri doğru koşma hızı
    public float laneDistance = 3f;      // Şeritler arası mesafe
    public float laneChangeSpeed = 10f;  // Şerit değiştirme hız
    [Header("Zorluk Ayarları")]
    public float speedIncreaseRate = 0.2f; // Saniyede hız ne kadar artck
    public float maxSpeed = 30f;           // oyunun ulasacagı max hız
    [Header("Fizik Ayarları")]
    public float jumpForce = 7f;  //zıplama
    private bool isGrounded = true; //zemine temas
    private Rigidbody rb;
    private BoxCollider col;
    private Vector3 originalColSize;
    private Vector3 originalColCenter;
    private int desiredLane = 1;         // 0: Sol, 1: Orta, 2: Sağ Oyuna ortadan başla
    private bool isGameOver = false; //oyun bitti mi?
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        originalColSize = col.size;
        originalColCenter = col.center;
    }
    void Update()
    {
        if (isGameOver)//oyun bittiyse dur
            return;
        // mevcut hız max hızdan küçükse hızı artır
        if (forwardSpeed < maxSpeed)
        {
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
        }
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
        // Zıplama Mekaniği (Space, Yukarı Ok veya W)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            anim.SetTrigger("Jump");//animsyn
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Eğilme Mekaniği (Aşağı Ok veya S)

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            anim.SetTrigger("Roll");

            // Transformu değil, sadece çarpışma kutusunu (Collider) küçült
            col.size = new Vector3(originalColSize.x, originalColSize.y / 2f, originalColSize.z);
            // Kutunun havada kalmaması için merkez noktasını biraz aşağı kaydır
            col.center = new Vector3(originalColCenter.x, originalColCenter.y - (originalColSize.y / 4f), originalColCenter.z);

            rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            // Tuşu bırakınca Collider'ı eski haline döndür
            col.size = originalColSize;
            col.center = originalColCenter;
        }
        // Hedef şeridin X pozisyonunu hesapla
        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 1) targetX = 0f;
        else if (desiredLane == 2) targetX = laneDistance;
        // Y eksenine (zıplamaya) müdahale etmemek için sadece X'i yumuşatıyoruz
        float newX = Mathf.Lerp(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        // Karakterin Z pozisyonunu int yap skora gönder
        int currentScore = Mathf.RoundToInt(transform.position.z);
        UIManager.instance.UpdateScore(currentScore);
    }
    // çarpışma fonk
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))//yere degme
        {
            isGrounded = true;
        }
        // Engele çarpma kontrolu
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            anim.SetTrigger("Die");
            isGameOver = true;
            Debug.Log("GAME OVER! Bir engele çarptın.");
            UIManager.instance.ShowGameOver();
        }
    }
}