using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Zemin Kontrolü")]
    public LayerMask groundLayer;
    [Tooltip("Raycast'in ayak tabanından ne kadar aşağı ineceğini belirler. 0.15 genelde bug'a girmiyor.")]
    public float rayDistance = 0.15f;

    [Header("Can Ayarları")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isInvincible = false;
    private Animator anim;

    [Header("Ses Ayarları")]
    [Tooltip("Inspector'dan freecoinsound sesini atamayı unutma")]
    public AudioClip coinSound;
    [Range(0f, 1f)]
    public float coinVolume = 0.8f;

    private AudioSource audioSource;

    [Tooltip("Hasar yeme sesi")]
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float hitVolume = 0.4f;

    [Header("Koşma Sesi Ayarları")]
    [Tooltip("Koşma loop sesi (freesoudrunning)")]
    public AudioClip runSound;
    [Range(0f, 1f)]
    public float runVolume = 0.5f;

    private AudioSource runAudioSource;

    [Header("Yuvarlanma (Roll) Sesi")]
    public AudioClip rollSound;
    [Range(0f, 1f)]
    public float rollVolume = 0.5f;

    [Header("Zıplama (Jump) Sesi")]
    public AudioClip jumpSound;
    [Range(0f, 1f)]
    public float jumpVolume = 0.5f;

    [Header("Hareket Ayarları")]
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 15f;

    [Header("Zorluk Ayarları")]
    public float speedIncreaseRate = 0.2f;
    public float maxSpeed = 100f;

    [Header("Fizik & Yerçekimi Ayarları (Subway Surfers Hissiyatı)")]
    public float jumpForce = 8.5f;
    public float extraGravityOnGround = 60f;
    public float extraGravityFalling = 55f;
    public float groundSlamForce = 25f;
    public float maxHeight = 5f;

    [Tooltip("Zıplama anında Raycast'in ne kadar süreyle pasif kalacağı (saniye)")]
    public float jumpCooldown = 0.15f;

    [Header("Yuvarlanma (Roll) Ayarları")]
    public float rollDuration = 0.65f;

    private bool isGrounded = true;
    private bool isJumping = false;
    private float jumpCooldownTimer = 0f;
    private Rigidbody rb;

    private CapsuleCollider col;
    private float originalColHeight;
    private Vector3 originalColCenter;

    private int desiredLane = 1;
    private bool isGameOver = false;
    private int collectedCoins = 0;

    // Yuvarlanma durumu kontrolü
    private bool isRolling = false;

    // Oyun başladı mı kontrolü
    private bool isGameStarted = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        col = GetComponent<CapsuleCollider>();
        originalColHeight = col.height;
        originalColCenter = col.center;

        // SFX'ler için AudioSource componentini alıyoruz
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // Koşma sesi sürekli çalacağı için arkada ayrı bir AudioSource oluşturduk
        runAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource.clip = runSound;
        runAudioSource.loop = true;
        runAudioSource.playOnAwake = false;
        runAudioSource.volume = runVolume;
    }

    void Update()
    {
        if (!isGameStarted || isGameOver)
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

        // Zıplama kontrolü
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded && !isJumping)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            isJumping = true;
            jumpCooldownTimer = jumpCooldown;

            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, jumpVolume);
            }
        }

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isRolling)
        {
            StartCoroutine(RollRoutine());
        }

        int currentScore = Mathf.RoundToInt(transform.position.z);
        UIManager.instance.UpdateScore(currentScore);
    }

    void FixedUpdate()
    {
        // Yere değip değmediğimizi Raycast ile anlıyoruz
        int layerMask = LayerMask.GetMask("Ground");
        Vector3 capsuleBottom = transform.TransformPoint(col.center + Vector3.down * (col.height / 2f));
        Vector3 rayStart = capsuleBottom + Vector3.up * 0.05f;
        RaycastHit hit;

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
            isGrounded = false;
        }
        else
        {
            float currentRayDistance = isJumping ? rayDistance : rayDistance * 1.5f;

            if (Physics.Raycast(rayStart, Vector3.down, out hit, currentRayDistance, layerMask))
            {
                isGrounded = true;

                if (!isJumping && hit.normal.y > 0.9f && rb.linearVelocity.y > 0f)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                }

                if (rb.linearVelocity.y <= 0.1f)
                {
                    isJumping = false;
                }
            }
            else
            {
                isGrounded = false;
            }
        }

        // Yokuş yukarı fırlama bug'ını çözmek için
        if (!isGrounded && !isJumping)
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
        }

        if (isGameOver)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // 3 şerit arası geçiş (x ekseninde hesaplama)
        float targetX = 0f;
        if (desiredLane == 0) targetX = -laneDistance;
        else if (desiredLane == 1) targetX = 0f;
        else if (desiredLane == 2) targetX = laneDistance;

        if (Mathf.Abs(targetX - transform.position.x) < 0.05f)
        {
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }

        float moveX = (targetX - transform.position.x) * laneChangeSpeed;
        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, forwardSpeed);

        // Oyuna kendi yerçekimimizi uyguluyoruz ki karakter yavaşça süzülmesin
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * extraGravityOnGround, ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * extraGravityFalling, ForceMode.Acceleration);
        }

        // Karakter uzaya uçmasın diye max Y sınırı
        if (transform.position.y > maxHeight)
        {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
        }

        bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded;

        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isGrounded", isGrounded);

        // Koşma sesi durum kontrolü
        if (runAudioSource != null)
        {
            if (isGrounded && !isGameOver && isGameStarted && !isRolling)
            {
                if (!runAudioSource.isPlaying)
                {
                    runAudioSource.Play();
                }
            }
            else
            {
                if (runAudioSource.isPlaying)
                {
                    runAudioSource.Stop();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        if (collision.gameObject.CompareTag("Obstacle") && !isInvincible)
        {
            currentHealth--;
            UIManager.instance.UpdateHealth(currentHealth);

            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound, hitVolume);
            }

            if (currentHealth <= 0)
            {
                anim.SetTrigger("Die");
                isGameOver = true;

                // Ölünce koşma sesi dursun
                if (runAudioSource != null && runAudioSource.isPlaying)
                {
                    runAudioSource.Stop();
                }

                int finalScore = Mathf.RoundToInt(transform.position.z);
                SaveManager.Instance.SaveGame(finalScore, collectedCoins);
                Debug.Log("GAME OVER");
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

            if (audioSource != null && coinSound != null)
            {
                audioSource.PlayOneShot(coinSound, coinVolume);
            }
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

        if (audioSource != null && rollSound != null)
        {
            audioSource.PlayOneShot(rollSound, rollVolume);
        }

        if (!isGrounded)
        {
            isJumping = false;
            jumpCooldownTimer = 0f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -groundSlamForce, rb.linearVelocity.z);
        }

        // Yuvarlanırken engellere çarpmamak için collider'ı küçültüyoruz
        float targetHeight = originalColHeight / 3f;
        col.height = targetHeight;

        float bottomY = originalColCenter.y - (originalColHeight / 2f);
        col.center = new Vector3(originalColCenter.x, bottomY + (targetHeight / 2f), originalColCenter.z);

        yield return new WaitForSeconds(rollDuration);

        col.height = originalColHeight;
        col.center = originalColCenter;
        isRolling = false;
    }

    public void StartRunning()
    {
        isGameStarted = true;
    }

    private void OnDrawGizmos()
    {
        if (col == null) col = GetComponent<CapsuleCollider>();
        if (col == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Vector3 capsuleBottom = transform.TransformPoint(col.center + Vector3.down * (col.height / 2f));
        Vector3 rayStart = capsuleBottom + Vector3.up * 0.05f;
        Vector3 rayEnd = rayStart + Vector3.down * rayDistance;

        Gizmos.DrawLine(rayStart, rayEnd);
        Gizmos.DrawWireSphere(rayEnd, 0.03f);
    }
}