using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UpcomingMatchesHandler
{
    #region Tags
    private const string URI_HLTV_MATCHES = "https://www.hltv.org/matches";

    private static string tagUpcomingMatches = "upcomingMatchesWrapper";
    private static string tagTeams = "upcomingMatch ";
    private static string tagFirstTeamID = "team1";
    private static string tagSecondTeamID = "team2";
    private static string tagEventNameLine = "matchEventLogoContainer";
    private static string tagEventName = "img alt=\"";
    private static string tagDateLine = "class=\"matchDayHeadline\">";
    private static string tagDate = "</span>";
    private static string tagTimeLine = "matchTime";
    private static string tagTime = "</div>";
    private static string tagFormatLine = "matchMeta";
    private static string tagFormat = "</div>";
    #endregion


    public static List<UpcomingMatch> GetUpcomingMatches()
    {
        string html = HTMLUtility.GetResponse(URI_HLTV_MATCHES);

        if (!Directory.Exists(Application.streamingAssetsPath)) { Directory.CreateDirectory(Application.streamingAssetsPath); }
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "upcomingMatches.txt"), html);


        List<UpcomingMatch> upcomingMatches = new List<UpcomingMatch>();
        UpcomingMatch lastUpcomingMatch = new UpcomingMatch();
        string[] strings = html.Split('\n');

        int dateYear = 0, dateMonth = 0, dateDay = 0;

        bool isMatchesContainer = false;
        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains(tagUpcomingMatches))
            {
                isMatchesContainer = true;
                continue;
            }

            if (isMatchesContainer)
            {
                if (strings[i].Contains(tagDateLine))
                {
                    int index = strings[i].IndexOf(tagDate) - 10;

                    char[] temp = new char[4];

                    for (int p = index; p < index + 4; p++)
                    {
                        temp[p - index] = strings[i][p];
                    }

                    string year = new string(temp);
                    int.TryParse(year, out dateYear);


                    index = strings[i].IndexOf(tagDate) - 5;

                    temp = new char[2];

                    for (int p = index; p < index + 2; p++)
                    {
                        temp[p - index] = strings[i][p];
                    }

                    string month = new string(temp);
                    int.TryParse(month, out dateMonth);


                    index = strings[i].IndexOf(tagDate) - 2;

                    temp = new char[2];

                    for (int p = index; p < index + 2; p++)
                    {
                        temp[p - index] = strings[i][p];
                    }

                    string day = new string(temp);
                    int.TryParse(day, out dateDay);
                }

                if (strings[i].Contains(tagTeams))
                {
                    if (lastUpcomingMatch.FirstTeamID > 0 && lastUpcomingMatch.SecondTeamID > 0)
                    {
                        upcomingMatches.Add(lastUpcomingMatch);
                    }
                    lastUpcomingMatch = new UpcomingMatch();


                    if (strings[i].Contains(tagFirstTeamID) && strings[i].Contains(tagSecondTeamID))
                    {
                        int index = strings[i].IndexOf(tagFirstTeamID);
                        int finishIndex = strings[i].IndexOf("\"", index + tagFirstTeamID.Length + 2);

                        char[] temp = new char[finishIndex - index - tagFirstTeamID.Length - 2];

                        for (int p = index + tagFirstTeamID.Length + 2; p < finishIndex; p++)
                        {
                            temp[p - index - tagFirstTeamID.Length - 2] = strings[i][p];
                        }

                        string stringfirstTeamID = new string(temp);

                        int.TryParse(stringfirstTeamID, out lastUpcomingMatch.FirstTeamID);


                        index = strings[i].IndexOf(tagSecondTeamID);
                        finishIndex = strings[i].IndexOf("\"", index + tagSecondTeamID.Length + 2);

                        temp = new char[finishIndex - index - tagSecondTeamID.Length - 2];

                        for (int p = index + tagSecondTeamID.Length + 2; p < finishIndex; p++)
                        {
                            temp[p - index - tagSecondTeamID.Length - 2] = strings[i][p];
                        }

                        string stringSecondTeamID = new string(temp);

                        int.TryParse(stringSecondTeamID, out lastUpcomingMatch.SecondTeamID);
                    }
                }

                if (strings[i].Contains(tagTimeLine))
                {
                    int finishIndex = strings[i].IndexOf(tagTime);
                    int index = finishIndex - 5;

                    char[] temp = new char[5];

                    for (int p = index; p < finishIndex; p++)
                    {
                        temp[p - index] = strings[i][p];
                    }

                    string dateTime = new string(temp);

                    System.DateTime tempDateTime;
                    System.DateTime.TryParse(dateTime, out tempDateTime);

                    lastUpcomingMatch.DateTime = new System.DateTime(dateYear, dateMonth, dateDay, tempDateTime.Hour, tempDateTime.Minute, tempDateTime.Second);
                    lastUpcomingMatch.DateTime = lastUpcomingMatch.DateTime.AddHours(2);
                }

                if (strings[i].Contains(tagEventNameLine))
                {
                    int index = strings[i].IndexOf(tagEventName);
                    int finishIndex = strings[i].IndexOf("\"", index + tagEventName.Length);

                    char[] temp = new char[finishIndex - index - tagEventName.Length];

                    for (int p = index + tagEventName.Length; p < finishIndex; p++)
                    {
                        temp[p - index - tagEventName.Length] = strings[i][p];
                    }

                    lastUpcomingMatch.EventName = new string(temp);
                }

                if (strings[i].Contains(tagFormatLine))
                {
                    int index = strings[i].IndexOf(tagFormat) - 1;

                    string format = new string(new char[] { strings[i][index] });

                    int.TryParse(format, out lastUpcomingMatch.Format);
                }
            }
        }

        return upcomingMatches;
    }
}