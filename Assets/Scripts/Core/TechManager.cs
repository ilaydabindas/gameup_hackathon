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

        activeTimers[tech.techId] = tech.maxTurns;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUp();
        }

        Debug.Log($"[Ar-Ge] {tech.techName} aktifleştirildi!");
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
            }
        }
    }

    public void ResetTechs()
    {
        activeTimers.Clear();
    }
}