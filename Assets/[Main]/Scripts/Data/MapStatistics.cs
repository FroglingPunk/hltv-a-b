using System;

[Serializable]
public class MapStatistics
{
    public EMap Map;

    public int TimesPlayed;
    public int Wins;
    public int Draws;
    public int Losses;
    public int TotalRoundsPlayed;
    public int RoundsWon;
    public int CTRoundWinPercent;
    public int TRoundWinPercent;
}