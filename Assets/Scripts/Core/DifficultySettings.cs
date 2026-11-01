using UnityEngine;

public enum DifficultyLevel { Easy, Balanced, Hard }

[System.Serializable]
public struct DifficultyParams
{
    public int initCarbon;
    public int initBudget;
    public int initSupport;
    public int passiveCarbonPerTurn;
    public int upkeepCostPerTurn;
    public float cardTimerSeconds;
    public int winThreshold;
}

public static class DifficultyConfig
{
    public static DifficultyParams GetParams(DifficultyLevel level)
    {
        switch (level)
        {
            case DifficultyLevel.Easy:
                return new DifficultyParams {
                    initCarbon = 42, initBudget = 75, initSupport = 72,
                    passiveCarbonPerTurn = 1, upkeepCostPerTurn = 1,
                    cardTimerSeconds = 20.0f, winThreshold = 55
                };
            case DifficultyLevel.Hard:
                return new DifficultyParams {
                    initCarbon = 56, initBudget = 55, initSupport = 58,
                    passiveCarbonPerTurn = 4, upkeepCostPerTurn = 4,
                    cardTimerSeconds = 12.5f, winThreshold = 45
                };
            default: // Balanced (Varsayılan)
                return new DifficultyParams {
                    initCarbon = 48, initBudget = 65, initSupport = 65,
                    passiveCarbonPerTurn = 2, upkeepCostPerTurn = 2,
                    cardTimerSeconds = 16.0f, winThreshold = 50
                };
        }
    }
}