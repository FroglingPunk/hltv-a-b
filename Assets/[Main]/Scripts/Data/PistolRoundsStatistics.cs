using System;

[Serializable]
public class PistolRoundsStatistics
{
    public int PistolRounds;
    public int PistolRoundsWon;
    public EMap Map;

    public double TotalPistolRoundWinPercent => Math.Round((double)PistolRoundsWon / PistolRounds * 100D, 2);

    public override string ToString()
    {
        return "\nTotal ::: " + PistolRounds + "\nWon ::: " + PistolRoundsWon + "\nPistol Round Win Rate : " + TotalPistolRoundWinPercent.ToString();
    }
}