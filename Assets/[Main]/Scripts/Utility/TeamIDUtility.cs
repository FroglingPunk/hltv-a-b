using System;
using System.IO;
using UnityEngine;

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

                        for (int i = 0; i < lines.Length; i++)
                        {
                            teamsDB[i] = JsonUtility.FromJson<TeamData>(lines[i]);
                        }
                    }
                }
                else
                {
                    string url = @"https://www.hltv.org/stats/teams?startDate=2018-01-01&endDate=" + SimpleDateTime.Now.CorrectForURL() + @"&minMapCount=0";
                    string html = HTMLUtility.GetResponse(url);

                    teamsDB = HLTVParcer.GetAllTeams(html).ToArray();

                    using (StreamWriter stream = File.CreateText(TeamsDBPath))
                    {
                        string json;

                        for (int i = 0; i < teamsDB.Length - 1; i++)
                        {
                            json = JsonUtility.ToJson(teamsDB[i]);
                            stream.WriteLine(json);
                        }

                        json = JsonUtility.ToJson(teamsDB[teamsDB.Length - 1]);
                        stream.Write(json);
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