using System;

[Serializable]
public class MapStatistics
{
    public string Map;

    public int PistolRounds;
    public int PistolRoundsWon;

    public float TotalPistolRoundWinPercent;
    public float CTPistolRoundWinPercent;
    public float TPistolRoundWinPercent;

    public override string ToString()
    {
        return Map + "\nPistol Round Win Rate : " + TotalPistolRoundWinPercent.ToString();
    }
}