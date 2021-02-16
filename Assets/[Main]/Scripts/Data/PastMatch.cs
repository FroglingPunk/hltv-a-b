using System;
using System.Collections.Generic;

[Serializable]
public class PastMatch
{
    public string TeamWinner;
    public string TeamLoser;
    public SimpleDateTime DateTime;
    public int MatchID;
    public string EventName;
    public int Format;

    public List<EMap> TeamWinnerPick = new List<EMap>();
    public List<EMap> TeamLoserPick = new List<EMap>();
    public EMap LeftOverMap;

    public List<PastMapID> PastMapsID = new List<PastMapID>();
    public List<PistolRoundsResults> PistolRoundsResults = new List<PistolRoundsResults>();

    public List<PistolRoundsStatistics> TeamWinnerPistolRoundStats = new List<PistolRoundsStatistics>();
    public List<PistolRoundsStatistics> TeamLoserPistolRoundStats = new List<PistolRoundsStatistics>();

    public List<MapStatistics> TeamWinnerMapStats = new List<MapStatistics>();
    public List<MapStatistics> TeamLoserMapStats = new List<MapStatistics>();

    public int TeamWinnerGlobalRating;
    public int TeamLoserGlobalRating;


    public override string ToString()
    {
        return "WINNER ::: " + TeamWinner + "   LOSER ::: " + TeamLoser + "   DATE ::: " + DateTime + "   MATCH ID ::: " + MatchID + "   EVENT ::: " + EventName + "   FORMAT ::: BO" + Format;
    }


    public string BuildURLToMatch()
    {
        string url = string.Empty;

        url = "https://www.hltv.org/matches/";
        url += (MatchID + "/");
        url += (TeamIDUtility.CorrectTeamNameForURL(TeamWinner) + "-vs-" + TeamIDUtility.CorrectTeamNameForURL(TeamLoser));
        url += "-" + TeamIDUtility.CorrectTeamNameForURL(EventName);

        return url;
    }
}