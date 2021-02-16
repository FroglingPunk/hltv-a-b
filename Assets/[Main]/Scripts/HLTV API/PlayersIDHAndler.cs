using System.Collections.Generic;

public static class PlayersIDHAndler
{
    private static string tagPlayerID = "a href=\"/player/";
    private static string tagMatchLineupsStart = "lineups";
    private static string tagTeamID = "a href=\"/team/";


    public static List<int> GetTeampPlayersIDFromTeamOverviewPage(string teamOverviewPageHTML)
    {
        List<int> playersID = new List<int>();

        string[] strings = teamOverviewPageHTML.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains(tagPlayerID))
            {
                int startIndex = strings[i].IndexOf(tagPlayerID) + tagPlayerID.Length;
                int endIndex = strings[i].IndexOf('/', startIndex);

                int currentPlayerID = int.Parse(strings[i].Substring(startIndex, endIndex - startIndex));
                playersID.Add(currentPlayerID);
            }
        }

        return playersID;
    }

    public static Dictionary<int, List<int>> GetTeampPlayersIDFromMatchPage(string matchPageHTML)
    {
        Dictionary<int, List<int>> lineups = new Dictionary<int, List<int>>();

        bool isLineupZone = false;
        string[] strings = matchPageHTML.Split('\n');
        int currentTeamID = -1;

        for (int i = 0; i < strings.Length; i++)
        {
            if (!isLineupZone && strings[i].Contains(tagMatchLineupsStart))
            {
                isLineupZone = true;
            }

            if (isLineupZone)
            {
                if (strings[i].Contains(tagTeamID))
                {
                    int startIndex = strings[i].IndexOf(tagTeamID) + tagTeamID.Length;
                    int endIndex = strings[i].IndexOf('/', startIndex);
                    currentTeamID = int.Parse(strings[i].Substring(startIndex, endIndex - startIndex));
                    lineups.Add(currentTeamID, new List<int>());
                }

                if (strings[i].Contains(tagPlayerID))
                {
                    int startIndex = strings[i].IndexOf(tagPlayerID) + tagPlayerID.Length;
                    int endIndex = strings[i].IndexOf('/', startIndex);

                    if (currentTeamID == -1 || lineups[currentTeamID].Count >= 5)
                    {
                        if (lineups.Count == 2)
                        {
                            break;
                        }
                        currentTeamID = -1;
                    }
                    else
                    {
                        int playerID = int.Parse(strings[i].Substring(startIndex, endIndex - startIndex));
                        if (lineups[currentTeamID].Contains(playerID))
                        {
                            // в редких матчах (ооочень редких, тир миллион) может отсутствовать ссылка на профиль игрока
                            // пример
                            // https://www.hltv.org/matches/2345933/pride-vs-nofear5-dreamhack-showdown-winter-2020-europe
                            // можно менять способ парсинга, но проще забить :p
                            break;
                        }

                        lineups[currentTeamID].Add(playerID);
                    }
                }
            }
        }

        return lineups;
    }
}