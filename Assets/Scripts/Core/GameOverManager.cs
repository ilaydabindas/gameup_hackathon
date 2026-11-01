using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI reasonText;

    private void Start()
    {
        // Sahne her yüklendiğinde zamanın normal aktığından emin ol
        Time.timeScale = 1f;
    }

    public void ShowGameOver(string title, string reason)
    {
        gameOverPanel.SetActive(true);
        titleText.text = title;
        reasonText.text = reason;

        // Oyunu dondur
        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        Debug.Log("Yeniden Başlat tetiklendi!");
        Time.timeScale = 1f; // Zamanı başlat
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}