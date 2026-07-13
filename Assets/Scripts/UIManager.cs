using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

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
    public TMP_Dropdown graphicsDropdown; 

    [Header("URP Grafik Ayarları")]
    public RenderPipelineAsset[] qualityAssets;

    public static UIManager instance;

    // oyunun menüden mi yoksa restart'tan mı başladığını aklında tutacak değişken
    public static bool isRestarting = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Kaydedilen grafik ayarını oku (Kayıt yoksa varsayılan olarak Unitygüncel kalitesi çek)
        int defaultQuality = QualitySettings.GetQualityLevel();
        int savedQuality = PlayerPrefs.GetInt("SelectedQuality", defaultQuality);

        //  Dropdown arayüzündeki seçimi kayıttaki değere eşitle
        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = savedQuality;
            graphicsDropdown.RefreshShownValue();
        }

        //  Grafik ayarını fiziksel olarak uygula
        ApplyQualitySettings(savedQuality);

        if (isRestarting)
        {
            Time.timeScale = 1f;
            mainMenuPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            isRestarting = false;
        }
        else
        {
            Time.timeScale = 0f;
            mainMenuPanel.SetActive(true);
            gameOverPanel.SetActive(false);
        }

        UpdateMainMenuUI();
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        isRestarting = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

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

    // Arayüzden Dropdown tıklandığında çalışan fonksiyon
    public void SetQuality(int qualityIndex)
    {
        // Seçimi hafızaya kaydet
        PlayerPrefs.SetInt("SelectedQuality", qualityIndex);
        PlayerPrefs.Save();

        // Kaliteyi fiziksel olarak uygula
        ApplyQualitySettings(qualityIndex);
    }

    // Grafik ayarlarını uygulayan yardımcı fonksiyon
    private void ApplyQualitySettings(int qualityIndex)
    {
        if (qualityAssets != null && qualityIndex < qualityAssets.Length)
        {
            // Unity'nin kalite seviyesini değiştir (Low/Medium/High)
            QualitySettings.SetQualityLevel(qualityIndex, true);

            //  URP'nin aktif render motorunu kodla kesin olarak değiştir
            GraphicsSettings.defaultRenderPipeline = qualityAssets[qualityIndex];

            Debug.Log("Grafik Kalitesi Uygulandı: " + qualityAssets[qualityIndex].name);
        }
    }
}