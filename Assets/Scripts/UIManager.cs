using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // sahneyendenbaslta

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;   // Skor yazısı
    public GameObject gameOverPanel;    // Game Over panel
    public TextMeshProUGUI coinText; // altn
    public GameObject[] hearts;
    //script'e her yerden kolayca ulaş
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
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
}