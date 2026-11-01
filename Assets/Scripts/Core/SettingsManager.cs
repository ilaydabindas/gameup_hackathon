using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Aktif Ayarlar")]
    public DifficultyLevel currentDifficulty = DifficultyLevel.Balanced;
    public int totalTurns = 24;
    public bool isSoundEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- 1. ZORLUK AYARLARI ---
    public void SetDifficultyEasy()
    {
        currentDifficulty = DifficultyLevel.Easy;
        ApplyDifficulty();
    }

    public void SetDifficultyBalanced()
    {
        currentDifficulty = DifficultyLevel.Balanced;
        ApplyDifficulty();
    }

    public void SetDifficultyHard()
    {
        currentDifficulty = DifficultyLevel.Hard;
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {
        DifficultyParams dParams = DifficultyConfig.GetParams(currentDifficulty);
        Debug.Log($"[Ayarlar] Zorluk güncellendi: {currentDifficulty} (Başlangıç Karbon: {dParams.initCarbon})");
    }

    // --- 2. TUR UZUNLUĞU ---
    public void SetCampaignLength(int turns)
    {
        totalTurns = turns;
        Debug.Log($"[Ayarlar] Toplam tur sayısı belirlendi: {totalTurns}");
    }

    // --- 3. SES AÇMA / KAPATMA ---
    public void ToggleSound()
    {
        isSoundEnabled = !isSoundEnabled;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
        }

        Debug.Log($"[Ayarlar] Ses durumu: {(isSoundEnabled ? "AÇIK" : "KAPALI")}");
    }

    // --- 4. BÖLÜMÜ YENİDEN BAŞLATMA ---
    public void RestartCurrentLevel()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        Time.timeScale = 1f;
        // Buton animasyonunun ve olay döngüsünün bitmesi için karenin sonunu bekle
        yield return new WaitForEndOfFrame();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}