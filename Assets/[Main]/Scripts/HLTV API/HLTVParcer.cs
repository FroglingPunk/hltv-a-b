using System.Collections.Generic;

public static class HLTVParcer
{
    private static string tagPistolRoundsTotal = "Pistol rounds</span><span>";
    private static string tagPistolRoundsWon = "Pistol rounds won</span><span>";
    private static string tagTimesPlayed = "Times played</span><span>";
    private static string tagWindDrawsLosses = "Wins / draws / losses</span><span>";
    private static string tagTotalRoundsPlayed = "Total rounds played</span><span>";
    private static string tagRoundsWon = "Rounds won</span><span>";
    private static string tagCTRoundsWinPercent = "CT round win percent</span><span class=";
    private static string tagTRoundsWinPercent = "T round win percent</span><span class=";


    public static PistolRoundsStatistics GetPistolRoundStats(string html)
    {
        PistolRoundsStatistics stats = new PistolRoundsStatistics();

        string[] strings = html.Split('\n');

        for(int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains(tagPistolRoundsTotal))
            {
                int startIndex = strings[i].IndexOf(tagPistolRoundsTotal) + tagPistolRoundsTotal.Length;
                int endIndex = strings[i].IndexOf('<', startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.PistolRounds);
            }

            if (strings[i].Contains(tagPistolRoundsWon))
            {
                int startIndex = strings[i].IndexOf(tagPistolRoundsWon) + tagPistolRoundsWon.Length;
                int endIndex = strings[i].IndexOf('<', startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.PistolRoundsWon);
            }
        }

        return stats;
    }

    public static MapStatistics GetMapStats(string html)
    {
        MapStatistics stats = new MapStatistics();

        string[] strings = html.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains(tagTimesPlayed))
            {
                int startIndex = strings[i].IndexOf(tagTimesPlayed) + tagTimesPlayed.Length;
                int endIndex = strings[i].IndexOf("<", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.TimesPlayed);
            }

            if (strings[i].Contains(tagWindDrawsLosses))
            {
                int startIndex = strings[i].IndexOf(tagWindDrawsLosses) + tagWindDrawsLosses.Length;
                int endIndex = strings[i].IndexOf(" /", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.Wins);

                startIndex = endIndex + 3;
                endIndex = strings[i].IndexOf(" /", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.Draws);

                startIndex = endIndex + 3;
                endIndex = strings[i].IndexOf("<", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.Losses);
            }

            if (strings[i].Contains(tagTotalRoundsPlayed))
            {
                int startIndex = strings[i].IndexOf(tagTotalRoundsPlayed) + tagTotalRoundsPlayed.Length;
                int endIndex = strings[i].IndexOf("<", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.TotalRoundsPlayed);
            }

            if (strings[i].Contains(tagRoundsWon))
            {
                int startIndex = strings[i].IndexOf(tagRoundsWon) + tagRoundsWon.Length;
                int endIndex = strings[i].IndexOf("<", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.RoundsWon);
            }

            if (strings[i].Contains(tagCTRoundsWinPercent))
            {
                int startIndex = strings[i].IndexOf(">", strings[i].IndexOf(tagCTRoundsWinPercent) + tagCTRoundsWinPercent.Length) + 1;
                int endIndex = strings[i].IndexOf(".", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.CTRoundWinPercent);
            }

            if (strings[i].Contains(tagTRoundsWinPercent))
            {
                int startIndex = strings[i].IndexOf(">", strings[i].IndexOf(tagTRoundsWinPercent) + tagTRoundsWinPercent.Length) + 1;
                int endIndex = strings[i].IndexOf(".", startIndex);

                int.TryParse(strings[i].Substring(startIndex, endIndex - startIndex), out stats.TRoundWinPercent);
            }
        }

        return stats;
    }
   
    public static List<TeamData> GetAllTeams(string html)
    {
        List<TeamData> allTeamsData = new List<TeamData>();

        string[] strings = html.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            string dbg = "/stats/teams/";
            if (strings[i].Contains(dbg))
            {
                TeamData teamData = new TeamData();

                int index = strings[i].IndexOf(dbg);
                int finishIndex = strings[i].IndexOf("/", index + dbg.Length);


                char[] temp = new char[finishIndex - index - dbg.Length];

                for (int p = index + dbg.Length; p < finishIndex; p++)
                {
                    temp[p - index - dbg.Length] = strings[i][p];
                }

                string teamID = new string(temp);

                int.TryParse(teamID, out teamData.ID);

                if (strings[i].IndexOf("data-tooltip-id") > 0)
                {
                    int start = strings[i].IndexOf('>', strings[i].IndexOf("data-tooltip-id")) + 1;

                    string teamNameString = strings[i].Substring(start).Replace("</a></td>", "");

                    teamData.Name = teamNameString;

                    allTeamsData.Add(teamData);
                }
            }
        }

        return allTeamsData;
    }
}