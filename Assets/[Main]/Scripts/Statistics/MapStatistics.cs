using System;

[Serializable]
public class MapStatistics
{
    public string Map;

    public PistolRoundsStatistics PistolRoundsStatistics;

    public override string ToString()
    {
        return Map + "\nPistol Round Win Rate : " + PistolRoundsStatistics.TotalPistolRoundWinPercent.ToString();
    }
}