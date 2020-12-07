using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Polygon : MonoBehaviour
{
    public string teamName = "Astralis";
    public float suitableDelta = 10f;

    public bool GET_RESPONSE = false;
    public bool DB_UNPACK = false;
    public bool GET_TEAM_STATS = false;
    public bool GET_UPCOMING_MATCHES = false;
    public bool FIND_SUITABLE_MATCHES = false;

    public string uri;
    public string uriUpcomingMatches;

    void OnValidate()
    {
        if (GET_RESPONSE)
        {
            GET_RESPONSE = false;
            string html = HTMLUtility.GetResponse(uri);

            if (!Directory.Exists(Application.streamingAssetsPath)) { Directory.CreateDirectory(Application.streamingAssetsPath); }
            using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "html.txt")))
            {
                stream.Write(html);
            }

            List<TeamStatistics> allTeams = GetAllTeams(html);

            for (int i = 0; i < allTeams.Count; i++)
            {
                Debug.Log(allTeams[i].ToString());

                string htmlPath = @"https://www.hltv.org/team/" + allTeams[i].ID + @"/" + allTeams[i].Team + @"#tab-statsBox";
                allTeams[i].mapsStatistics = GetPistolRoundStats(HTMLUtility.GetResponse(htmlPath)).ToArray();
            }

            using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "db.txt")))
            {
                for (int i = 0; i < allTeams.Count; i++)
                {
                    string json = JsonUtility.ToJson(allTeams[i]);
                    stream.WriteLine(json);
                }
            }
        }

        if (GET_TEAM_STATS)
        {
            GET_TEAM_STATS = false;

            TeamStatistics teamStatistics = GetTeamStatistics(teamName);

            Debug.Log(teamStatistics);
            for (int p = 0; p < teamStatistics.mapsStatistics.Length; p++)
            {
                Debug.Log(teamStatistics.mapsStatistics[p].ToString());
            }
        }

        if (DB_UNPACK)
        {
            DB_UNPACK = false;

            using (StreamReader stream = File.OpenText(Path.Combine(Application.streamingAssetsPath, "db.txt")))
            {
                string json = stream.ReadToEnd();

                string[] lines = json.Split('\n');

                List<TeamStatistics> teamStatistics = new List<TeamStatistics>(lines.Length);

                for (int i = 0; i < lines.Length - 1; i++)
                {
                    teamStatistics.Add(JsonUtility.FromJson<TeamStatistics>(lines[i]));
                    Debug.Log(teamStatistics[i]);
                    for (int p = 0; p < teamStatistics[i].mapsStatistics.Length; p++)
                    {
                        Debug.Log(teamStatistics[i].mapsStatistics[p]);
                    }
                }
            }
        }

        if (GET_UPCOMING_MATCHES)
        {
            GET_UPCOMING_MATCHES = false;

            List<UpcomingMatch> upcomingMatches = UpcomingMatchesHandler.GetUpcomingMatches();

            for (int i = 0; i < upcomingMatches.Count; i++)
            {
                Debug.Log(upcomingMatches[i]);
            }
        }

        if (FIND_SUITABLE_MATCHES)
        {
            FIND_SUITABLE_MATCHES = false;

            string html = HTMLUtility.GetResponse(uri);
            List<UpcomingMatch> upcomingMatches = UpcomingMatchesHandler.GetUpcomingMatches();

            for (int i = 0; i < upcomingMatches.Count; i++)
            {
                TeamStatistics firstTeam = GetTeamStatistics(html, upcomingMatches[i].FirstTeamID);
                TeamStatistics secondTeam = GetTeamStatistics(html, upcomingMatches[i].SecondTeamID);

                for (int p = 0; p < maps.Length; p++)
                {
                    string currentMap = maps[p];

                    MapStatistics firstTeamMapStats = firstTeam.GetMapStatistics(currentMap);
                    MapStatistics secondTeamMapStats = secondTeam.GetMapStatistics(currentMap);

                    if (firstTeamMapStats != null && secondTeamMapStats != null)
                    {
                        if (Mathf.Abs(firstTeamMapStats.TotalPistolRoundWinPercent - secondTeamMapStats.TotalPistolRoundWinPercent) > suitableDelta)
                        {
                            Debug.Log(firstTeam.Team + " VS " + secondTeam.Team + " BO" + upcomingMatches[i].Format + " on " + upcomingMatches[i].DateTime + " ::: " + currentMap);
                        }
                    }
                }
            }
        }
    }

    IEnumerator Start()
    {
        string html = HTMLUtility.GetResponse(uri);

        if (!Directory.Exists(Application.streamingAssetsPath)) { Directory.CreateDirectory(Application.streamingAssetsPath); }
        using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "html.txt")))
        {
            stream.Write(html);
        }

        List<TeamStatistics> allTeams = GetAllTeams(html);

        for (int i = 0; i < allTeams.Count; i++)
        {
            Debug.Log(allTeams[i].ToString());
            yield return new WaitForSeconds(0.1f);
            string htmlPath = @"https://www.hltv.org/team/" + allTeams[i].ID + @"/" + allTeams[i].Team + @"#tab-statsBox";
            allTeams[i].mapsStatistics = GetPistolRoundStats(HTMLUtility.GetResponse(htmlPath)).ToArray();
        }

        using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "db.txt")))
        {
            for (int i = 0; i < allTeams.Count; i++)
            {
                string json = JsonUtility.ToJson(allTeams[i]);
                stream.WriteLine(json);
            }
        }
    }


    static private string[] maps = new[]
     {
        "Overpass",
        "Mirage",
        "Train",
        "Inferno",
        "Dust2",
        "Nuke",
        "Vertigo",

        "Cache"
    };


    public static TeamStatistics GetTeamStatistics(string teamName)
    {
        teamName = teamName.ToLower();

        using (StreamReader stream = File.OpenText(Path.Combine(Application.streamingAssetsPath, "db.txt")))
        {
            string json = stream.ReadToEnd();

            string[] lines = json.Split('\n');

            TeamStatistics teamStatistics = null;

            for (int i = 0; i < lines.Length - 1; i++)
            {
                teamStatistics = JsonUtility.FromJson<TeamStatistics>(lines[i]);
                if (teamStatistics.Team.ToLower() == teamName)
                {
                    return teamStatistics;
                }
            }
        }

        return null;
    }

    static public List<MapStatistics> GetPistolRoundStats(string html)
    {
        List<int> PRstringsID = new List<int>();
        string[] strings = html.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains("Pistolround"))
            {
                PRstringsID.Add(i);
            }
        }

        List<MapStatistics> mapsStatistics = new List<MapStatistics>();
        for (int i = 0; i < PRstringsID.Count; i++)
        {
            MapStatistics mapStats = new MapStatistics();

            string PRstring = strings[PRstringsID[i] + 1];
            PRstring = PRstring.Trim();

            PRstring = PRstring.Remove(0, 5);
            PRstring = PRstring.Remove(PRstring.Length - 7, 7);

            PRstring = PRstring.Replace(".", ",");

            mapStats.TotalPistolRoundWinPercent = float.Parse(PRstring);

            string mapString = strings[PRstringsID[i] + 4];
            for (int p = 0; p < maps.Length; p++)
            {
                if (mapString.Contains(maps[p]))
                {
                    mapStats.Map = maps[p];
                    break;
                }
            }

            mapsStatistics.Add(mapStats);
        }

        return mapsStatistics;
    }

    static public List<TeamStatistics> GetAllTeams(string html)
    {
        List<TeamStatistics> teams = new List<TeamStatistics>();

        string[] strings = html.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            string dbg = "/stats/teams/";
            if (strings[i].Contains(dbg))
            {
                TeamStatistics teamStats = new TeamStatistics();

                int index = strings[i].IndexOf(dbg);
                int finishIndex = strings[i].IndexOf("/", index + dbg.Length);


                char[] temp = new char[finishIndex - index - dbg.Length];

                for (int p = index + dbg.Length; p < finishIndex; p++)
                {
                    temp[p - index - dbg.Length] = strings[i][p];
                }

                string teamID = new string(temp);

                int.TryParse(teamID, out teamStats.ID);

                if (strings[i].IndexOf("data-tooltip-id") > 0)
                {
                    int start = strings[i].IndexOf('>', strings[i].IndexOf("data-tooltip-id")) + 1;

                    string teamNameString = strings[i].Substring(start).Replace("</a></td>", "");

                    teamStats.Team = teamNameString;

                    teams.Add(teamStats);
                }
            }
        }

        return teams;
    }

    static public TeamStatistics GetTeamStatistics(string html, int id)
    {
        TeamStatistics team = new TeamStatistics();

        string[] strings = html.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            string dbg = "/stats/teams/";
            if (strings[i].Contains(dbg))
            {
                int index = strings[i].IndexOf(dbg);
                int finishIndex = strings[i].IndexOf("/", index + dbg.Length);

                char[] temp = new char[finishIndex - index - dbg.Length];

                for (int p = index + dbg.Length; p < finishIndex; p++)
                {
                    temp[p - index - dbg.Length] = strings[i][p];
                }

                string teamID = new string(temp);

                int.TryParse(teamID, out team.ID);

                if (team.ID != id)
                {
                    continue;
                }

                if (strings[i].IndexOf("data-tooltip-id") > 0)
                {
                    int start = strings[i].IndexOf('>', strings[i].IndexOf("data-tooltip-id")) + 1;

                    string teamNameString = strings[i].Substring(start).Replace("</a></td>", "");

                    team.Team = teamNameString;
                }

                break;
            }
        }

        string htmlPath = @"https://www.hltv.org/team/" + team.ID + @"/" + team.Team + @"#tab-statsBox";
        team.mapsStatistics = GetPistolRoundStats(HTMLUtility.GetResponse(htmlPath)).ToArray();

        return team;
    }
}