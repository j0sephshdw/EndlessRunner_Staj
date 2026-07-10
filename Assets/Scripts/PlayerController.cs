using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isInvincible = false; // Çarptıktan sonraki dokunulmazlık
    private Animator anim;
    [Header("Hareket Ayarları")]
    public float forwardSpeed = 10f;     // İleri doğru koşma hızı
    public float laneDistance = 3f;      // Şeritler arası mesafe
    public float laneChangeSpeed = 10f;  // Şerit değiştirme hız
    [Header("Zorluk Ayarları")]
    public float speedIncreaseRate = 0.2f; // Saniyede hız ne kadar artck
    public float maxSpeed = 100f;           // oyunun ulasacagı max hız
    [Header("Fizik Ayarları")]
    public float jumpForce = 7f;  //zıplama
    private bool isGrounded = true; //zemine temas
    private Rigidbody rb;
    private BoxCollider col;
    private Vector3 originalColSize;
    private Vector3 originalColCenter;
    private int desiredLane = 1;         // 0: Sol, 1: Orta, 2: Sağ Oyuna ortadan başla
    private bool isGameOver = false; //oyun bitti mi
    private int collectedCoins = 0; // Toplanan altın sayısı
    void Start()
    {
        currentHealth = maxHealth;
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
            StartCoroutine(RollRoutine());
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

        // Engele çarpma kontrolü
        if (collision.gameObject.CompareTag("Obstacle") && !isInvincible)
        {
            currentHealth--; // Canı 1 azalt
            UIManager.instance.UpdateHealth(currentHealth); // Arayüzden kalbi sil

            if (currentHealth <= 0)
            {
                // Can bittiyse tamamen öl
                anim.SetTrigger("Die");
                isGameOver = true;
                Debug.Log("GAME OVER! Canın bitti.");
                UIManager.instance.ShowGameOver();
            }
            else
            {
                // Can varsa hasar animasyonu oynat ve engeli yok et
                anim.SetTrigger("Hit");
                Destroy(collision.gameObject);

                // Kısa süreli dokunulmazlık başlat
                StartCoroutine(InvincibilityRoutine());
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            collectedCoins++; //  1 sltın artır
            UIManager.instance.UpdateCoin(collectedCoins); // UIa haber ver

            Destroy(other.gameObject); // Altınısil
        }
    }
    // Hasar aldıktan sonra 1 saniyelik dokunulmazlık süresi
    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1f); // 1 saniye bekle
        isInvincible = false;
    }
    // rollng ve Colliderı küçükltmeı
    private System.Collections.IEnumerator RollRoutine()
    {
        anim.SetTrigger("Roll");

        // Colliderı yarıya indir
        col.size = new Vector3(originalColSize.x, originalColSize.y / 2f, originalColSize.z);
        // Kutunun merkezini aşağı kaydır
        col.center = new Vector3(originalColCenter.x, originalColCenter.y - (originalColSize.y / 4f), originalColCenter.z);

        // Zıplarken eglrse hızlıc yere düş
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
        }

        // Animasyonun bitmesni bekle
        yield return new WaitForSeconds(1.15f);

        // Süre dolnca Collidereski haline döndür
        col.size = originalColSize;
        col.center = originalColCenter;
    }
}