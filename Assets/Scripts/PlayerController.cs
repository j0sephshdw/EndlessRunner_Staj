using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isInvincible = false;
    private Animator anim;

    [Header("Hareket Ayarları")]
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;

    [Header("Zorluk Ayarları")]
    public float speedIncreaseRate = 0.2f;
    public float maxSpeed = 100f;

    [Header("Fizik Ayarları")]
    public float jumpForce = 7f;
    public float maxHeight = 5f; // Karakterin max yükseklik sınırı
    private bool isGrounded = true;
    private Rigidbody rb;

    private CapsuleCollider col;
    private float originalColHeight;
    private Vector3 originalColCenter;

    private int desiredLane = 1;
    private bool isGameOver = false;
    private int collectedCoins = 0;

    // RAM Patlamasını Engelleyen Güvenlik Kilidi
    private bool isRolling = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        col = GetComponent<CapsuleCollider>();
        originalColHeight = col.height;
        originalColCenter = col.center;
    }

    void Update()
    {
        if (isGameOver)
            return;

        if (forwardSpeed < maxSpeed)
        {
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            anim.SetTrigger("Jump");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Zıplama öncesi dikey hızı sıfırla
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Eğer zaten eğilmiyorsak yeni Coroutine başlatabilirsin (RAM kilidi)
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isRolling)
        {
            StartCoroutine(RollRoutine());
        }

        int currentScore = Mathf.RoundToInt(transform.position.z);
        UIManager.instance.UpdateScore(currentScore);
    }

    void FixedUpdate()
    {
        if (isGameOver)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 1) targetX = 0f;
        else if (desiredLane == 2) targetX = laneDistance;

        // Yumuşatılmış sağ/sol hız hesaplaması
        float moveX = (targetX - transform.position.x) * laneChangeSpeed;

        // Fizik motoru ile ileri koşma
        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, forwardSpeed);

        // --- RAMPADAN UÇMAYI ENGELLE
        // Eğer karakter zıplamadıysa, onu aşağı çek
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * 50f, ForceMode.Acceleration);
        }

        // YENİ: MAKSİMUM YÜKSEKLİK KONTROLÜ
        if (transform.position.y > maxHeight)
        {
            // Karakteri zorla sınırda tut
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);

            // Yukarı doğru fırlama hızını sıfırla (hemen aşağı düşmeye başlamasını sağlar)
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Yere veya rampaya değdiğinde zıplama hakkını geri ver
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Untagged"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Obstacle") && !isInvincible)
        {
            currentHealth--;
            UIManager.instance.UpdateHealth(currentHealth);

            if (currentHealth <= 0)
            {
                anim.SetTrigger("Die");
                isGameOver = true;
                Debug.Log("GAME OVER! Canın bitti.");
                UIManager.instance.ShowGameOver();
            }
            else
            {
                anim.SetTrigger("Hit");
                Destroy(collision.gameObject);
                StartCoroutine(InvincibilityRoutine());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            collectedCoins++;
            UIManager.instance.UpdateCoin(collectedCoins);
            Destroy(other.gameObject);
        }
    }

    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1f);
        isInvincible = false;
    }

    private System.Collections.IEnumerator RollRoutine()
    {
        isRolling = true;
        anim.SetTrigger("Roll");

        // Boyu 3'te 1'ine indir yüksek engeller için
        float targetHeight = originalColHeight / 3f;
        col.height = targetHeight;

        // Kapsül küçülürken ayak tabanının havaya kalkmasını önle
        float bottomY = originalColCenter.y - (originalColHeight / 2f);
        col.center = new Vector3(originalColCenter.x, bottomY + (targetHeight / 2f), originalColCenter.z);

        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1.15f);

        // Süre bitince kapsülü orijinal boyutlarına ve merkezine geri döndür
        col.height = originalColHeight;
        col.center = originalColCenter;
        isRolling = false;
    }
}