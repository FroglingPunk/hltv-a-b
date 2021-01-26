﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public static class HLTVParcer
{
    public static void GetResponce(string uri)
    {
        string html = HTMLUtility.GetResponse(uri);

        if (!Directory.Exists(Application.streamingAssetsPath)) { Directory.CreateDirectory(Application.streamingAssetsPath); }
        using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "html.txt")))
        {
            stream.Write(html);
        }

        List<TeamData> allTeams = GetAllTeams(html);

        for (int i = 0; i < allTeams.Count; i++)
        {
            Debug.Log(allTeams[i].ToString());

            //string htmlPath = @"https://www.hltv.org/team/" + allTeams[i].ID + @"/" + allTeams[i].Name + @"#tab-statsBox";
            //allTeams[i].mapsStatistics = GetPistolRoundStats(HTMLUtility.GetResponse(htmlPath)).ToArray();
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

    public static void DBUnpack()
    {
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

    private static string[] maps = new[]
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


    //public static List<MapStatistics> GetPistolRoundStats(string html)
    //{
    //    List<int> PRstringsID = new List<int>();
    //    string[] strings = html.Split('\n');

    //    for (int i = 0; i < strings.Length; i++)
    //    {
    //        if (strings[i].Contains("Pistolround"))
    //        {
    //            PRstringsID.Add(i);
    //        }
    //    }

    //    List<MapStatistics> mapsStatistics = new List<MapStatistics>();
    //    for (int i = 0; i < PRstringsID.Count; i++)
    //    {
    //        MapStatistics mapStats = new MapStatistics();

    //        string PRstring = strings[PRstringsID[i] + 1];
    //        PRstring = PRstring.Trim();

    //        PRstring = PRstring.Remove(0, 5);
    //        PRstring = PRstring.Remove(PRstring.Length - 7, 7);

    //        PRstring = PRstring.Replace(".", ",");

    //        mapStats.PistolRoundsStatistics.TotalPistolRoundWinPercent = float.Parse(PRstring);

    //        string mapString = strings[PRstringsID[i] + 4];
    //        for (int p = 0; p < maps.Length; p++)
    //        {
    //            if (mapString.Contains(maps[p]))
    //            {
    //                mapStats.Map = maps[p];
    //                break;
    //            }
    //        }

    //        mapsStatistics.Add(mapStats);
    //    }

    //    return mapsStatistics;
    //}

    private static string tagPistolRoundsTotal = "Pistol rounds</span><span>";
    private static string tagPistolRoundsWon = "Pistol rounds won</span><span>";


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

                char[] charsID = new char[endIndex - startIndex];

                for (int p = startIndex; p < endIndex; p++)
                {
                    charsID[p - startIndex] = strings[i][p];
                }

                int.TryParse(new string(charsID), out stats.PistolRounds);
            }

            if (strings[i].Contains(tagPistolRoundsWon))
            {
                int startIndex = strings[i].IndexOf(tagPistolRoundsWon) + tagPistolRoundsWon.Length;
                int endIndex = strings[i].IndexOf('<', startIndex);

                char[] charsID = new char[endIndex - startIndex];

                for (int p = startIndex; p < endIndex; p++)
                {
                    charsID[p - startIndex] = strings[i][p];
                }

                int.TryParse(new string(charsID), out stats.PistolRoundsWon);
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