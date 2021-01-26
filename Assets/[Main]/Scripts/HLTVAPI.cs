using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HLTVAPI : MonoBehaviour
{
    // https://www.hltv.org/stats/lineup/map/34?lineup=19232&lineup=9410&lineup=11250&lineup=16870&lineup=11815&minLineupMatch=5&startDate=2000-01-24&endDate=2021-01-24

    public bool GET_RESPONSE = false;
    public bool DEBUG_TEAM_PLAYERS_ID = false;
    public bool DEBUG_TEAM_PLAYERS_ID_BY_TEAM_ID = false;
    public bool DEBUG_BUILD_URL = false;
    public bool GET_TEAM_NAME_BY_ID = false;
    public bool GET_PISTOL_STATS = false;
    public string uri;
    public string teamPageURI;
    public int teamID;
    public EMap map;




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
        }

        if (DEBUG_TEAM_PLAYERS_ID)
        {
            DEBUG_TEAM_PLAYERS_ID = false;

            string html = HTMLUtility.GetResponse(teamPageURI);
            List<int> teamPlayersID = PlayersIDHAndler.GetTeampPlayersID(html);
            for (int i = 0; i < teamPlayersID.Count; i++)
            {
                Debug.Log(teamPlayersID[i]);
            }
        }

        if (DEBUG_TEAM_PLAYERS_ID_BY_TEAM_ID)
        {
            DEBUG_TEAM_PLAYERS_ID_BY_TEAM_ID = false;

            string html = HTMLUtility.GetResponse(TeamIDUtility.BuildURLToTeamOverviewPage(TeamIDUtility.GetTeamData(teamID)));
            List<int> teamPlayersID = PlayersIDHAndler.GetTeampPlayersID(html);
            for (int i = 0; i < teamPlayersID.Count; i++)
            {
                Debug.Log(teamPlayersID[i]);
            }
        }

        if (DEBUG_BUILD_URL)
        {
            DEBUG_BUILD_URL = false;

            Debug.Log(BuildURL(EMap.Dust2, PlayersIDHAndler.GetTeampPlayersID(HTMLUtility.GetResponse(teamPageURI)).ToArray(),
                 DateTime.Now.Subtract(new TimeSpan(90, 0, 0, 0, 0)), DateTime.Now));
        }

        if (GET_TEAM_NAME_BY_ID)
        {
            GET_TEAM_NAME_BY_ID = false;

            string teamName = TeamIDUtility.GetTeamData(teamID).Name;

            Debug.Log("TEAM NAME ::: " + teamName + "\nURL CORRECT NAME ::: " + TeamIDUtility.CorrectTeamNameForURL(teamName));
        }

        if (GET_PISTOL_STATS)
        {
            GET_PISTOL_STATS = false;

            string teamOverviewPageURL = TeamIDUtility.BuildURLToTeamOverviewPage(TeamIDUtility.GetTeamData(teamID));

            string url = BuildURL(map, PlayersIDHAndler.GetTeampPlayersID(HTMLUtility.GetResponse(teamOverviewPageURL)).ToArray(),
                 DateTime.Now.Subtract(new TimeSpan(90, 0, 0, 0, 0)), DateTime.Now);

            string html = HTMLUtility.GetResponse(url);

            PistolRoundsStatistics stats = HLTVParcer.GetPistolRoundStats(html);

            Debug.Log(stats);
        }
    }



    public static string BuildURL(EMap map, int[] playersID, DateTime startDate, DateTime endDate)
    {
        string url = string.Empty;

        url = @"https://www.hltv.org/stats/lineup/map/" + map.GetMapID() + "?";

        for (int i = 0; i < playersID.Length; i++)
        {
            url += ("lineup=" + playersID[i] + "&");
        }

        url += ("minLineupMatch=" + playersID.Length + "&");

        url += ("startDate=" + startDate.CorrectForURL() + "&");
        url += ("endDate=" + endDate.CorrectForURL());

        return url;
    }
}

public static class TeamIDUtility
{
    private static string TeamsDBPath => Path.Combine(Application.streamingAssetsPath, "Teams.txt");

    private static TeamData[] teamsDB = null;
    private static TeamData[] TeamsDB
    {
        get
        {
            if (teamsDB == null)
            {
                if (File.Exists(TeamsDBPath))
                {
                    using (StreamReader stream = File.OpenText(TeamsDBPath))
                    {
                        string json = stream.ReadToEnd();

                        string[] lines = json.Split('\n');

                        teamsDB = new TeamData[lines.Length];

                        for (int i = 0; i < lines.Length - 1; i++)
                        {
                            teamsDB[i] = JsonUtility.FromJson<TeamData>(lines[i]);
                        }
                    }
                }
                else
                {
                    string html = HTMLUtility.GetResponse("https://www.hltv.org/stats/teams?startDate=2020-10-25&endDate=2021-01-25&minMapCount=10");

                    teamsDB = HLTVParcer.GetAllTeams(html).ToArray();

                    using (StreamWriter stream = File.CreateText(TeamsDBPath))
                    {
                        for (int i = 0; i < teamsDB.Length; i++)
                        {
                            string json = JsonUtility.ToJson(teamsDB[i]);
                            stream.WriteLine(json);
                        }
                    }
                }
            }

            return teamsDB;
        }
    }


    public static TeamData GetTeamData(int id)
    {
        for (int i = 0; i < TeamsDB.Length; i++)
        {
            if (TeamsDB[i].ID == id)
            {
                return TeamsDB[i];
            }
        }

        return null;
    }

    public static string CorrectTeamNameForURL(string teamName)
    {
        return teamName.Replace(' ', '-');
    }

    public static string BuildURLToTeamOverviewPage(this TeamData teamData)
    {
        string url = string.Empty;

        url = "https://www.hltv.org/team/";
        url += (teamData.ID + "/");
        url += (CorrectTeamNameForURL(teamData.Name));

        return url;
    }
}


[Serializable]
public class TeamData
{
    public string Name;
    public int ID;
}