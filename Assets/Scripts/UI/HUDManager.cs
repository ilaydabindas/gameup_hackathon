using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField] private Slider carbonSlider;
    [SerializeField] private Slider budgetSlider;
    [SerializeField] private Slider supportSlider;
    [SerializeField] private TextMeshProUGUI turnText;

    private void Awake() { Instance = this; }

    // Bu metodu 3. Kişi (Sistemci) GameManager içinden çağıracak
    public void UpdateMetrics(float carbon, float budget, float support, int currentTurn, int maxTurns)
    {
        carbonSlider.value = carbon / 100f;
        budgetSlider.value = budget / 100f;
        supportSlider.value = support / 100f;
        turnText.text = $"TUR {currentTurn}/{maxTurns}";
    }
}