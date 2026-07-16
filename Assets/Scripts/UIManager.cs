using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Audio;

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

    [Header("Ayarlar Arayüzü")]
    public GameObject settingsPanel;
    public TMP_Dropdown graphicsDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioMixer audioMixer;

    [Header("URP Grafik Ayarları")]
    public RenderPipelineAsset[] qualityAssets;

    public static UIManager instance;
    public static bool isRestarting = false;

    private Resolution[] resolutions;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // --- ÇÖZÜNÜRLÜK SİSTEMİ ---
        resolutions = Screen.resolutions;
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();

            int currentMonitorResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                // Monitörün o anki doğal çözünürlüğünü tespit et
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentMonitorResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);

            int finalResolutionIndex;

            // Eğer daha önce hiç çözünürlük ayarı kaydedilmemişse
            if (!PlayerPrefs.HasKey("ResolutionIndex"))
            {
                // İlk açılışta monitörün doğal çözünürlüğünü varsayılan yap
                finalResolutionIndex = currentMonitorResolutionIndex;

                PlayerPrefs.SetInt("ResolutionIndex", finalResolutionIndex);
                PlayerPrefs.Save();

                Resolution firstRes = resolutions[finalResolutionIndex];
                Screen.SetResolution(firstRes.width, firstRes.height, Screen.fullScreen);
            }
            else
            {
                finalResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");

                // Index out of bounds hatasını önlemek için monitör değişimi kontrolü
                if (finalResolutionIndex >= resolutions.Length)
                {
                    finalResolutionIndex = currentMonitorResolutionIndex;
                }

                Resolution savedRes = resolutions[finalResolutionIndex];
                Screen.SetResolution(savedRes.width, savedRes.height, Screen.fullScreen);
            }

            resolutionDropdown.value = finalResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        // --- GRAFİK AYARLARINI YÜKLE ---
        int defaultQuality = QualitySettings.GetQualityLevel();
        int savedQuality = PlayerPrefs.GetInt("SelectedQuality", defaultQuality);

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = savedQuality;
            graphicsDropdown.RefreshShownValue();
        }
        ApplyQualitySettings(savedQuality);

        // --- SES AYARLARINI YÜKLE ---
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // Oyun açıldığında seslerin doğru seviyede başlaması için AudioMixer'a aktar
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(savedMusic) * 20);
            audioMixer.SetFloat("SFX", Mathf.Log10(savedSFX) * 20);
        }

        // --- OYUN BAŞLANGIÇ DURUMU (RESTART / MENÜ) ---
        if (isRestarting)
        {
            Time.timeScale = 1f;
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            isRestarting = false;

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null) player.StartRunning();
        }
        else
        {
            Time.timeScale = 0f;
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        UpdateMainMenuUI();
    }

    // --- TEMEL OYUN FONKSİYONLARI ---

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.StartRunning();
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

    // --- AYARLAR MENÜSÜ ---

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // --- AYAR KAYIT FONKSİYONLARI ---

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        ApplyQualitySettings(qualityIndex);

        PlayerPrefs.SetInt("SelectedQuality", qualityIndex);
        PlayerPrefs.Save();
    }

    private void ApplyQualitySettings(int qualityIndex)
    {
        if (qualityAssets != null && qualityIndex < qualityAssets.Length)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
            GraphicsSettings.defaultRenderPipeline = qualityAssets[qualityIndex];
            Debug.Log("Grafik Kalitesi Uygulandı: " + qualityAssets[qualityIndex].name);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}