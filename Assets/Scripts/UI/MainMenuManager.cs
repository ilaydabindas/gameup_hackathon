using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Yüklenecek Sahne")]
    [SerializeField] private string gameplaySceneName = "GameScene"; // Kart sahnenin adı

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OpenTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}