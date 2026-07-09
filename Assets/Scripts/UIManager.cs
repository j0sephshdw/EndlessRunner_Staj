using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // sahneyendenbaslta

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;   // Skor yazısı
    public GameObject gameOverPanel;    // Game Over panel
    public TextMeshProUGUI coinText; // altn
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
}