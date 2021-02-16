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
            List<int> teamPlayersID = PlayersIDHAndler.GetTeampPlayersIDFromTeamOverviewPage(html);
            for (int i = 0; i < teamPlayersID.Count; i++)
            {
                Debug.Log(teamPlayersID[i]);
            }
        }

        if (DEBUG_TEAM_PLAYERS_ID_BY_TEAM_ID)
        {
            DEBUG_TEAM_PLAYERS_ID_BY_TEAM_ID = false;

            string html = HTMLUtility.GetResponse(TeamIDUtility.BuildURLToTeamOverviewPage(TeamIDUtility.GetTeamData(teamID)));
            List<int> teamPlayersID = PlayersIDHAndler.GetTeampPlayersIDFromTeamOverviewPage(html);
            for (int i = 0; i < teamPlayersID.Count; i++)
            {
                Debug.Log(teamPlayersID[i]);
            }
        }

        if (DEBUG_BUILD_URL)
        {
            DEBUG_BUILD_URL = false;

            Debug.Log(BuildURL(map, PlayersIDHAndler.GetTeampPlayersIDFromTeamOverviewPage(HTMLUtility.GetResponse(TeamIDUtility.BuildURLToTeamOverviewPage(TeamIDUtility.GetTeamData(teamID)))).ToArray(),
                 SimpleDateTime.Now.Subtract(90), SimpleDateTime.Now));
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

            string url = BuildURL(map, PlayersIDHAndler.GetTeampPlayersIDFromTeamOverviewPage(HTMLUtility.GetResponse(teamOverviewPageURL)).ToArray(),
                 SimpleDateTime.Now.Subtract(90), SimpleDateTime.Now);

            string html = HTMLUtility.GetResponse(url);

            PistolRoundsStatistics stats = HLTVParcer.GetPistolRoundStats(html);

            Debug.Log(stats);
        }
    }



    public static string BuildURL(EMap map, int[] playersID, SimpleDateTime startDate, SimpleDateTime endDate)
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