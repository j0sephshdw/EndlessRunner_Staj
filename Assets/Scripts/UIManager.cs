using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Oyun İçi Arayüz")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public GameObject[] hearts;
    public GameObject gameOverPanel;

    [Header("Ana Menü Arayüzü")]
    public GameObject mainMenuPanel;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI totalCoinsText;

    public static UIManager instance;

    //oyunun menüden mi yoksa restart'tan mı başladığını aklında tutacak değişken
    public static bool isRestarting = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (isRestarting)
        {
            // Eğer "Yeniden Başla" butonundan geliyorsak: Menüyü atla, direkt oyna
            Time.timeScale = 1f;
            mainMenuPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            isRestarting = false; // Bir sonraki tur için sıfırla
        }
        else
        {
            // Eğer oyunu ilk kez açıyorsak veya "Menüye Dön" dediysek: Menüyü göster
            Time.timeScale = 0f;
            mainMenuPanel.SetActive(true);
            gameOverPanel.SetActive(false);
        }

        UpdateMainMenuUI();
    }

    // "Başla" Butonu
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // "Yeniden Başla" Butonu
    public void RestartGame()
    {
        isRestarting = true; // save Sahne açılınca menüyü atla
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // YENİ: "Menüye Dön" Butonu
    public void GoToMenu()
    {
        isRestarting = false; // save Sahne açılınca menüyü göster
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // "Çıkış" Butonu
    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkılıyor...");
        Application.Quit();
    }

    public void UpdateScore(int score) { scoreText.text = "Skor: " + score; }
    public void UpdateCoin(int coinCount) { coinText.text = "Altın: " + coinCount.ToString(); }

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth) hearts[i].SetActive(true);
            else hearts[i].SetActive(false);
        }
    }

    public void ShowGameOver() { gameOverPanel.SetActive(true); }

    public void UpdateMainMenuUI()
    {
        if (SaveManager.Instance != null)
        {
            if (highScoreText != null) highScoreText.text = "Rekor: " + SaveManager.Instance.playerData.highScore.ToString();
            if (totalCoinsText != null) totalCoinsText.text = "Kasadaki Altın: " + SaveManager.Instance.playerData.totalCoins.ToString();
        }
    }
}