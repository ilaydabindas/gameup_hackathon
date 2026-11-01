using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [Header("Mini-Game Prefabları")]
    public GameObject[] miniGamePrefabs; 

    [Header("Hedef Alan")]
    public Transform cardSpawnPoint; 

    private int turnCounter = 0;
    private int targetTurn = 3;
    private GameObject activeMiniGameInstance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetNewTargetTurn();
    }

    private void SetNewTargetTurn()
    {
        targetTurn = Random.Range(3, 5); // 3 veya 4 turda bir
    }

    // Minigame sırası geldiyse true döner ve oyunu açar
    public bool CheckAndTriggerMiniGame()
    {
        if (activeMiniGameInstance != null) return false;

        turnCounter++;

        if (turnCounter >= targetTurn)
        {
            TriggerRandomMiniGame();
            turnCounter = 0;
            SetNewTargetTurn();
            return true;
        }

        return false;
    }

    private void TriggerRandomMiniGame()
    {
        if (miniGamePrefabs == null || miniGamePrefabs.Length == 0 || cardSpawnPoint == null) return;

        int randomIndex = Random.Range(0, miniGamePrefabs.Length);
        GameObject selectedPrefab = miniGamePrefabs[randomIndex];

        activeMiniGameInstance = Instantiate(selectedPrefab, cardSpawnPoint.parent);
        
        RectTransform rect = activeMiniGameInstance.GetComponent<RectTransform>();
        
        if (rect != null)
        {
            // Tam ortaya sabitle ve stretch modundan kurtar
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }
    }

    // Minigame başarıyla bittiğinde çağrılır
    public void CloseMiniGame()
    {
        if (activeMiniGameInstance != null)
        {
            Destroy(activeMiniGameInstance);
        }

        // Sıradaki kartı getirmesi için GameManager'ı devam ettir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeAfterMiniGame();
        }
    }
}