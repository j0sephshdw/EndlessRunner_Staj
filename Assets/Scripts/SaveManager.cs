using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public int highScore = 0;
    public int totalCoins = 0;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath;
    public GameData playerData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cihazdaki yerel dosya yolunu belirle
            saveFilePath = Path.Combine(Application.persistentDataPath, "player_save.json");

            // Oyun açılır açılmaz eski kayıtları yükle
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- OYUNU KAYDET ---
    public void SaveGame(int currentScore, int coinsCollectedThisRun)
    {
        // Rekor kontrolü yap
        if (currentScore > playerData.highScore)
        {
            playerData.highScore = currentScore;
            Debug.Log("Tebrikler! Yeni Rekor: " + currentScore);
        }

        // Toplanan altınları kasaya ekle
        playerData.totalCoins += coinsCollectedThisRun;

        // Objeyi JSON Metnine Dönüştür
        string jsonText = JsonUtility.ToJson(playerData, true);

        // JSON verisini cihaza fiziksel dosya olarak yaz
        File.WriteAllText(saveFilePath, jsonText);

        Debug.Log("Oyun Verileri Güvenle Kaydedildi. Yol: " + saveFilePath);
    }

    // --- KAYDI YÜKLE ---
    public void LoadGame()
    {
        // Eğer cihazda daha önceden oluşturulmuş bir save dosyası varsa oku
        if (File.Exists(saveFilePath))
        {
            string jsonText = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<GameData>(jsonText);
            Debug.Log("Eski kayıtlar başarıyla yüklendi.");
        }
        else
        {
            // İlk defa oynanıyorsa sıfır bir save nesnesi oluştur
            playerData = new GameData();
            Debug.Log("Kayıt dosyası bulunamadı, yeni profil oluşturuldu.");
        }
    }
}