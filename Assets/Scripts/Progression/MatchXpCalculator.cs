using UnityEngine;

public static class MatchXpCalculator
{
    public static int CalculateXp(int score, int linesCleared, int combos, bool watchedAdBonus = false)
    {
        int xp = 35;

        xp += linesCleared * 8;
        xp += combos * 12;
        xp += Mathf.FloorToInt(score / 500f);

        if (watchedAdBonus)
            xp = Mathf.RoundToInt(xp * 1.5f);

        return Mathf.Clamp(xp, 35, 180);
    }
}