using System.Collections.Generic;
using UnityEngine;

public class TechManager : MonoBehaviour
{
    public static TechManager Instance { get; private set; }

    [SerializeField] private List<TechData> availableTechs;
    private Dictionary<string, int> activeTimers = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool BuyTechnology(TechData tech)
    {
        if (IsTechActive(tech.techId)) return false;
        if (GameManager.Instance != null && GameManager.Instance.budget < tech.cost) return false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.budget -= tech.cost;
        }

        activeTimers[tech.techId] = tech.maxTurns;

        if (tech.techId == "mirror" && GameManager.Instance != null)
        {
            GameManager.Instance.carbon = Mathf.Max(0, GameManager.Instance.carbon - 12);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUp();
        }

        return true;
    }

    public bool IsTechActive(string techId)
    {
        return activeTimers.ContainsKey(techId) && activeTimers[techId] > 0;
    }

    public int GetRemainingTurns(string techId)
    {
        return activeTimers.ContainsKey(techId) ? activeTimers[techId] : 0;
    }

    public void ProcessTurnTick()
    {
        List<string> keys = new List<string>(activeTimers.Keys);
        foreach (var key in keys)
        {
            if (activeTimers[key] > 0)
            {
                activeTimers[key]--;
                ApplyPassivePerTurnEffect(key);
            }
        }
    }

    private void ApplyPassivePerTurnEffect(string techId)
    {
        if (GameManager.Instance == null) return;

        switch (techId)
        {
            case "fusion":
                GameManager.Instance.carbon = Mathf.Max(0, GameManager.Instance.carbon - 4);
                GameManager.Instance.budget = Mathf.Min(100, GameManager.Instance.budget + 5);
                break;
            case "forest":
                GameManager.Instance.carbon = Mathf.Max(0, GameManager.Instance.carbon - 2);
                GameManager.Instance.support = Mathf.Min(100, GameManager.Instance.support + 4);
                break;
        }
    }

    public void ResetTechs()
    {
        activeTimers.Clear();
    }
}
