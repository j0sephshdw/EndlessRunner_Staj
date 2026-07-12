using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // sahneyendenbaslta

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;   // Skor yazısı
    public GameObject gameOverPanel;    // Game Over panel
    public TextMeshProUGUI coinText; // altn
    public GameObject[] hearts;

    // Ana menüde/başlangıçta görünecek metinler
    [Header("Ana Menü Kayıt Arayüzü")]
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI totalCoinsText;

    //script'e her yerden kolayca ulaş
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }

    // saveleri yazdır basladıgında
    private void Start()
    {
        UpdateMainMenuUI();
    }

    // Skoru güncelle
    public void UpdateScore(int score)
    {
        scoreText.text = "Skor: " + score;
    }

    //ölünce Game Over aç
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    // Yeniden Başlaya basınca
    public void RestartGame()
    {
        // Mevcut sahneyi baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Altın sayacını güncelle
    public void UpdateCoin(int coinCount)
    {
        coinText.text = "Altın: " + coinCount.ToString();
    }

    public void UpdateHealth(int currentHealth)
    {
        // canımızdan büyük olan kalpleri gizle
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].SetActive(true); // Canımız göster
            else
                hearts[i].SetActive(false); // Canımız yoksa gizle
        }
    }

    //SaveManager'dan verileri çekip metinlere yazdır
    public void UpdateMainMenuUI()
    {
        // SaveManager sahnde mi
        if (SaveManager.Instance != null)
        {
            // Eğer Unity üzerinden Text'ler bağlandıysa 
            if (highScoreText != null)
            {
                highScoreText.text = "Rekor: " + SaveManager.Instance.playerData.highScore.ToString();
            }

            if (totalCoinsText != null)
            {
                totalCoinsText.text = "Kasadaki Altın: " + SaveManager.Instance.playerData.totalCoins.ToString();
            }
        }
    }
}