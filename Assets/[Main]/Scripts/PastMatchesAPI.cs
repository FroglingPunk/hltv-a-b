using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class PastMatchesAPI : MonoBehaviour
{
    //https://www.hltv.org/results?startDate=2020-12-27&endDate=2021-01-27&map=de_dust2


    public bool GET_RESPONSE = false;
    public bool DEBUG_BUILD_URL = false;
    public bool DEBUG_TOTALWORK = false;

    public string uri;
    public EMap map;

    public SimpleDateTime totalWorkStartTime;
    public SimpleDateTime totalWorkEndTime;
    public int startOffset;
    public int matchesCount;


    void OnValidate()
    {
        if (GET_RESPONSE)
        {
            GET_RESPONSE = false;

            string html = HTMLUtility.GetResponse(uri);

            ////https://www.hltv.org/results?offset=7000&startDate=2020-01-01&endDate=2020-12-31
            //string tagMatchesCount = "<span class=\"pagination-data\">";
            //int matchesCount = 0;
            //string[] lines = html.Split('\n');
            //for (int i = 0; i < lines[i].Length; i++)
            //{
            //    if (lines[i].Contains(tagMatchesCount))
            //    {
            //        Debug.Log(i);
            //        int matchesCountStartIndex = lines[i].IndexOf(tagMatchesCount) + tagMatchesCount.Length + 11;
            //        int matchesCountEndIndex = lines[i].IndexOf(" <", matchesCountStartIndex);
            //        string matchesCountString = lines[i].Substring(matchesCountStartIndex, matchesCountEndIndex - matchesCountStartIndex);

            //        Debug.Log(matchesCountString);
            //        int.TryParse(matchesCountString, out matchesCount);
            //        break;
            //    }
            //}


            //Debug.Log("NO");
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "html.txt"), html);
        }

        if (DEBUG_BUILD_URL)
        {
            DEBUG_BUILD_URL = false;

            Debug.Log(BuildURL(map, totalWorkStartTime, totalWorkEndTime, startOffset));
        }

        //if (DEBUG_TOTALWORK)
        //{
        //    DEBUG_TOTALWORK = false;

        //    FUNC_TOTALWORK(HTMLUtility.GetResponse(BuildURL(map, SimpleDateTime.Now.Subtract(1), SimpleDateTime.Now.Subtract(1))));
        //}
    }

    // problems
    // добавить перелистывание страниц результатов

    //void Start()
    //{
    //    StartCoroutine(FUNC_TOTALWORK());
    //}

    public void StartParcer()
    {
        if (!ParserIsWork)
        {
            StartCoroutine(FUNC_TOTALWORK());
        }
    }

    public static string BuildURL(EMap? map, SimpleDateTime startDate, SimpleDateTime endDate, int offset)
    {       
        //https://www.hltv.org/results?offset=94&startDate=2020-01-01&endDate=2020-12-31

        string url = string.Empty;

        url = @"https://www.hltv.org/results";

        url += ("?offset=" + offset);
        url += ("&startDate=" + startDate.CorrectForURL());
        url += ("&endDate=" + endDate.CorrectForURL());

        if (map.HasValue)
        {
            url += ("&map=de_" + map.Value.ToString().ToLower());
        }

        return url;
    }


    private static Dictionary<string, int> monthWordToNumber = new Dictionary<string, int>
    {
        { "January", 1 },
        { "February", 2 },
        { "March", 3 },
        { "April", 4 },
        { "May", 5 },
        { "June", 6 },
        { "July", 7 },
        { "August", 8 },
        { "September", 9 },
        { "October", 10 },
        { "November", 11 },
        { "December", 12 }
    };


    private static string tagDay = ">Results for ";
    private static string tagWinnerTeam = "<div class=\"team team-won\">";
    private static string tagLoserTeam = "<div class=\"team \">";
    private static string tagMatchID = "a href=\"/matches/";
    private static string tagEventName = "<td class=\"event\"><img alt=\"";
    private static string tagPastMatchesListOver = "<div class=\"leftCol\">";
    private static string tagMatchFormat = "map-text\">";

    private static string tagMapVetoStart = "<div class=\"standard-box veto-box\">";


    public IEnumerator FUNC_TOTALWORK()
    {
        try
        {
            OnParserStart?.Invoke();
        }
        catch { }

        ParserIsWork = true;

        WaitForSeconds pause = new WaitForSeconds(5f);
        List<PastMatch> pastMatches = new List<PastMatch>();

        int currentOffset = startOffset;

        while (pastMatches.Count < matchesCount)
        {
            yield return pause;
            string resultsPageUrl = BuildURL(null, totalWorkStartTime, totalWorkEndTime, currentOffset);
            string html = HTMLUtility.GetResponse(resultsPageUrl);

            string[] lines = html.Split('\n');
            SimpleDateTime dateTime = null;
            PastMatch match = null;

            int matchesCountBefore = pastMatches.Count;

            for (int i = 0; i < lines.Length && pastMatches.Count < matchesCount; i++)
            {
                if (lines[i].Contains(tagPastMatchesListOver))
                {
                    break;
                }

                if (lines[i].Contains(tagDay))
                {
                    dateTime = new SimpleDateTime();

                    #region month
                    int monthWordStartIndex = lines[i].IndexOf(tagDay) + tagDay.Length;
                    int monthWordEndIndex = lines[i].IndexOf(" ", monthWordStartIndex);

                    char[] monthWord = new char[monthWordEndIndex - monthWordStartIndex];

                    for (int p = monthWordStartIndex; p < monthWordEndIndex; p++)
                    {
                        monthWord[p - monthWordStartIndex] = lines[i][p];
                    }

                    dateTime.Month = monthWordToNumber[new string(monthWord)];
                    #endregion

                    #region day
                    int dayWordStartIndex = monthWordEndIndex + 1;
                    int dayWordEndIndex = lines[i].IndexOf("th", dayWordStartIndex);
                    if (dayWordEndIndex == -1)
                    {
                        dayWordEndIndex = lines[i].IndexOf("st", dayWordStartIndex);
                    }
                    if (dayWordEndIndex == -1)
                    {
                        dayWordEndIndex = lines[i].IndexOf("nd", dayWordStartIndex);
                    }
                    if (dayWordEndIndex == -1)
                    {
                        dayWordEndIndex = lines[i].IndexOf("rd", dayWordStartIndex);
                    }

                    char[] dayWord = new char[dayWordEndIndex - dayWordStartIndex];
                    for (int p = dayWordStartIndex; p < dayWordEndIndex; p++)
                    {
                        dayWord[p - dayWordStartIndex] = lines[i][p];
                    }

                    dateTime.Day = int.Parse(new string(dayWord));
                    #endregion

                    #region year
                    int yearWordStartIndex = dayWordEndIndex + 3;
                    int yearWordEndIndex = yearWordStartIndex + 4;

                    char[] yearWord = new char[4];
                    for (int p = yearWordStartIndex; p < yearWordEndIndex; p++)
                    {
                        yearWord[p - yearWordStartIndex] = lines[i][p];
                    }

                    dateTime.Year = int.Parse(new string(yearWord));
                    #endregion
                }

                if (dateTime != null)
                {
                    if (lines[i].Contains(tagMatchID))
                    {
                        int matchIDStartIndex = lines[i].IndexOf(tagMatchID) + tagMatchID.Length;
                        int matchIDEndIndex = lines[i].IndexOf("/", matchIDStartIndex);
                        string matchIDString = lines[i].Substring(matchIDStartIndex, matchIDEndIndex - matchIDStartIndex);

                        match = new PastMatch { DateTime = new SimpleDateTime(dateTime) };
                        int.TryParse(matchIDString, out match.MatchID);
                    }

                    if (lines[i].Contains(tagLoserTeam))
                    {
                        int loserTeamStartIndex = lines[i].IndexOf(tagLoserTeam) + tagLoserTeam.Length;
                        int loserTeamEndIndex = lines[i].IndexOf("<", loserTeamStartIndex);
                        match.TeamLoser = lines[i].Substring(loserTeamStartIndex, loserTeamEndIndex - loserTeamStartIndex);
                    }

                    if (lines[i].Contains(tagWinnerTeam))
                    {
                        int winnerTeamStartIndex = lines[i].IndexOf(tagWinnerTeam) + tagWinnerTeam.Length;
                        int winnerTeamEndIndex = lines[i].IndexOf("<", winnerTeamStartIndex);
                        match.TeamWinner = lines[i].Substring(winnerTeamStartIndex, winnerTeamEndIndex - winnerTeamStartIndex);
                    }

                    if (lines[i].Contains(tagEventName))
                    {
                        int eventNameStartIndex = lines[i].IndexOf(tagEventName) + tagEventName.Length;
                        int eventNameEndIndex = lines[i].IndexOf("\"", eventNameStartIndex);
                        match.EventName = lines[i].Substring(eventNameStartIndex, eventNameEndIndex - eventNameStartIndex);
                    }

                    if (lines[i].Contains(tagMatchFormat))
                    {
                        // если матч бо1, на сайте указывается название карты, в остальных случаях указывается формат(bo3, bo5, etc.)
                        int matchFormatStartIndex = lines[i].IndexOf(tagMatchFormat) + tagMatchFormat.Length;
                        if (lines[i].Substring(matchFormatStartIndex, 2) == "bo")
                        {
                            match.Format = int.Parse(lines[i].Substring(matchFormatStartIndex + 2, 1));
                        }
                        else
                        {
                            match.Format = 1;
                        }
                        pastMatches.Add(match);
                        match = null;
                    }
                }
            }

            if (pastMatches.Count == matchesCountBefore)
            {
                break;
            }

            currentOffset += 100;
        }

        for (int i = 0; i < pastMatches.Count; i++)
        {
            Dictionary<EMap, string> mapsPageHTML = new Dictionary<EMap, string>();
            string matchPageHTML;

            yield return pause;

            try
            {
                string pastMatchURL = pastMatches[i].BuildURLToMatch();
                matchPageHTML = HTMLUtility.GetResponse(pastMatchURL);

                pastMatches[i].PastMapsID = GET_PAST_MAPS_ID(matchPageHTML);
            }
            catch
            {
                continue;
            }


            for (int p = 0; p < pastMatches[i].PastMapsID.Count; p++)
            {
                yield return pause;
                try
                {
                    string urlToPastMap = BUILD_URL_TO_PAST_MAPS(pastMatches[i].PastMapsID[p].ID, pastMatches[i].TeamWinner, pastMatches[i].TeamLoser);
                    mapsPageHTML.Add(pastMatches[i].PastMapsID[p].Map, HTMLUtility.GetResponse(urlToPastMap));
                }
                catch
                {
                    continue;
                }
            }

            Dictionary<int, List<int>> lineups = new Dictionary<int, List<int>>();
            try
            {
                lineups = PlayersIDHAndler.GetTeampPlayersIDFromMatchPage(matchPageHTML);
                bool errorInLineup = false;
                foreach (int teamID in lineups.Keys)
                {
                    if (lineups[teamID].Count < 5)
                    {
                        errorInLineup = true;
                        break;
                    }
                }
                if (errorInLineup)
                {
                    Debug.LogWarning("ERROR IN LINEUP ::: " + pastMatches[i].ToString());
                    continue;
                }
            }
            catch
            {
                continue;
            }

            Dictionary<string, string> teamsOverviewPageHTML = new Dictionary<string, string>();
            Dictionary<EMap, string> teamWinnerMapStatsPageHTML = new Dictionary<EMap, string>();
            Dictionary<EMap, string> teamLoserMapStatsPageHTML = new Dictionary<EMap, string>();

            foreach (int teamID in lineups.Keys)
            {

                TeamData teamData = null;
                bool isTeamWinner = false;
                try
                {
                    teamData = TeamIDUtility.GetTeamData(teamID);
                    isTeamWinner = (teamData.Name == pastMatches[i].TeamWinner);

                    int deltaDays = (pastMatches[i].DateTime.DayOfWeek() - DayOfWeek.Monday);
                }
                catch
                {
                    continue;
                }

                yield return pause;
                try
                {
                    string teamPageURL = TeamIDUtility.BuildURLToTeamOverviewPage(teamData);
                    string teamPageHTML = HTMLUtility.GetResponse(teamPageURL);
                    teamsOverviewPageHTML.Add(teamData.Name, teamPageHTML);
                }
                catch
                {
                    continue;
                }


                for (int p = 0; p < pastMatches[i].PastMapsID.Count; p++)
                {
                    string url;

                    try
                    {
                        url = HLTVAPI.BuildURL(pastMatches[i].PastMapsID[p].Map, lineups[teamID].ToArray(), pastMatches[i].DateTime.Subtract(90), pastMatches[i].DateTime.Subtract(1));
                    }
                    catch
                    {
                        continue;
                    }
                    yield return pause;
                    string statsPageHTML = HTMLUtility.GetResponse(url);

                    try
                    {
                        if (isTeamWinner)
                        {
                            teamWinnerMapStatsPageHTML.Add(pastMatches[i].PastMapsID[p].Map, statsPageHTML);
                        }
                        else
                        {
                            teamLoserMapStatsPageHTML.Add(pastMatches[i].PastMapsID[p].Map, statsPageHTML);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            try
            {
                DETAILER_PAST_MATCH(pastMatches[i], matchPageHTML, mapsPageHTML, teamsOverviewPageHTML, teamWinnerMapStatsPageHTML, teamLoserMapStatsPageHTML);
            }
            catch
            {
                try
                {
                    OnMatchParsingError?.Invoke(pastMatches[i]);
                }
                catch { }

                Debug.LogError("ERROR ::: " + pastMatches[i].ToString());
                continue;
            }

            if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "Past Matches")))
            {
                Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "Past Matches"));
            }

            string json = JsonUtility.ToJson(pastMatches[i]);
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "Past Matches", "Match " + pastMatches[i].MatchID + ".txt"), json);

            try
            {
                PastMatch pMatch = pastMatches[i];
                OnMatchParsingResultRecorded?.Invoke(pMatch);
            }
            catch { }

            Debug.Log(pastMatches[i].ToString());
        }

        try
        {
            OnParserFinish?.Invoke();
        }
        catch { }

        ParserIsWork = false;
    }

    public bool ParserIsWork { get; private set; }
    public event Action OnParserStart;
    public event Action OnParserFinish;
    public event Action<PastMatch> OnMatchParsingResultRecorded;
    public event Action<PastMatch> OnMatchParsingError;

    public void DETAILER_PAST_MATCH(PastMatch match, string matchPageHTML, Dictionary<EMap, string> mapsPageHTML, Dictionary<string, string> teamsOverviewPageHTML, Dictionary<EMap, string> teamWinnerMapStatsPageHTML, Dictionary<EMap, string> teamLoserMapStatsPageHTML)
    {
        string[] lines = matchPageHTML.Split('\n');
        bool isMapVetoZone = false;

        string tagTeamWinnerPick = (match.TeamWinner + " picked ");
        string tagTeamLoserPick = (match.TeamLoser + " picked ");
        string tagLeftOverMap = "<div>7. ";

        for (int i = 0; i < lines.Length; i++)
        {
            if (!isMapVetoZone && lines[i].Contains(tagMapVetoStart))
            {
                isMapVetoZone = true;
            }
            else if (isMapVetoZone)
            {
                if (lines[i].Contains(tagLeftOverMap))
                {
                    int leftOverMapStartIndex = lines[i].IndexOf(tagLeftOverMap) + tagLeftOverMap.Length;
                    int leftOverMapEndIndex = lines[i].IndexOf(" was", leftOverMapStartIndex);
                    string leftOverMapString = lines[i].Substring(leftOverMapStartIndex, leftOverMapEndIndex - leftOverMapStartIndex);

                    match.LeftOverMap = MapsUtility.StringToEMap(leftOverMapString);
                    break;
                }

                if (lines[i].Contains(tagTeamWinnerPick))
                {
                    int teamWinnerPickStartIndex = lines[i].IndexOf(tagTeamWinnerPick) + tagTeamWinnerPick.Length;
                    int teamWinnerPickEndIndex = lines[i].IndexOf("<", teamWinnerPickStartIndex);
                    string teamWinnerPickString = lines[i].Substring(teamWinnerPickStartIndex, teamWinnerPickEndIndex - teamWinnerPickStartIndex);

                    match.TeamWinnerPick.Add(MapsUtility.StringToEMap(teamWinnerPickString));
                }

                if (lines[i].Contains(tagTeamLoserPick))
                {
                    int teamLoserPickStartIndex = lines[i].IndexOf(tagTeamLoserPick) + tagTeamLoserPick.Length;
                    int teamLoserPickEndIndex = lines[i].IndexOf("<", teamLoserPickStartIndex);
                    string teamLoserPickString = lines[i].Substring(teamLoserPickStartIndex, teamLoserPickEndIndex - teamLoserPickStartIndex);

                    match.TeamLoserPick.Add(MapsUtility.StringToEMap(teamLoserPickString));
                }
            }
        }

        foreach (EMap map in mapsPageHTML.Keys)
        {
            PistolRoundsResults pistolRoundsResults = new PistolRoundsResults();
            pistolRoundsResults.Map = map;
            Dictionary<string, int> pistolRoundsScore = MAP_PISTOL_WINNERS(mapsPageHTML[map]);
            foreach (string team in pistolRoundsScore.Keys)
            {
                bool isTeamWinner = (team == match.TeamWinner);

                if (isTeamWinner)
                {
                    pistolRoundsResults.TeamWinnerScore = pistolRoundsScore[team];
                }
                else
                {
                    pistolRoundsResults.TeamLoserScore = pistolRoundsScore[team];
                }
            }

            match.PistolRoundsResults.Add(pistolRoundsResults);
        }

        Dictionary<int, List<int>> lineups = new Dictionary<int, List<int>>();

        lineups = PlayersIDHAndler.GetTeampPlayersIDFromMatchPage(matchPageHTML);

        foreach (string teamName in teamsOverviewPageHTML.Keys)
        {
            int deltaDays = (match.DateTime.DayOfWeek() - DayOfWeek.Monday);
            int rating = GET_RATING(teamsOverviewPageHTML[teamName], match.DateTime.Subtract(deltaDays));

            bool isTeamWinner = (teamName == match.TeamWinner);

            if (isTeamWinner)
            {
                match.TeamWinnerGlobalRating = rating;
            }
            else
            {
                match.TeamLoserGlobalRating = rating;
            }
        }

        foreach (EMap map in teamWinnerMapStatsPageHTML.Keys)
        {
            PistolRoundsStatistics pistolRoundStats = HLTVParcer.GetPistolRoundStats(teamWinnerMapStatsPageHTML[map]);
            pistolRoundStats.Map = map;
            MapStatistics mapStats = HLTVParcer.GetMapStats(teamWinnerMapStatsPageHTML[map]);
            mapStats.Map = map;

            match.TeamWinnerPistolRoundStats.Add(pistolRoundStats);
            match.TeamWinnerMapStats.Add(mapStats);
        }

        foreach (EMap map in teamLoserMapStatsPageHTML.Keys)
        {
            PistolRoundsStatistics pistolRoundStats = HLTVParcer.GetPistolRoundStats(teamLoserMapStatsPageHTML[map]);
            pistolRoundStats.Map = map;
            MapStatistics mapStats = HLTVParcer.GetMapStats(teamLoserMapStatsPageHTML[map]);
            mapStats.Map = map;

            match.TeamLoserPistolRoundStats.Add(pistolRoundStats);
            match.TeamLoserMapStats.Add(mapStats);
        }
    }


    //public IEnumerator FUNC_TOTALWORK(string html)
    //{
    //    string[] lines = html.Split('\n');
    //    SimpleDateTime dateTime = null;
    //    List<PastMatch> pastMatches = new List<PastMatch>();
    //    PastMatch match = null;

    //    //https://www.hltv.org/results?offset=7000&startDate=2020-01-01&endDate=2020-12-31
    //    //string tagMatchesCount = @"1 - 100 of ";
    //    //int matchesCount = 0;

    //    //for (int i = 0; i < lines[i].Length; i++)
    //    //{
    //    //    Debug.Log(i);
    //    //    if (lines[i].Contains(tagMatchesCount))
    //    //    {
    //    //        int matchesCountStartIndex = lines[i].IndexOf(tagMatchesCount) + tagMatchesCount.Length;
    //    //        int matchesCountEndIndex = lines[i].IndexOf(" <", matchesCountStartIndex);
    //    //        string matchesCountString = lines[i].Substring(matchesCountStartIndex, matchesCountEndIndex - matchesCountStartIndex);

    //    //        Debug.Log(matchesCountString);
    //    //        int.TryParse(matchesCountString, out matchesCount);
    //    //        break;
    //    //    }
    //    //}

    //    //yield break;

    //    try
    //    {
    //        for (int i = 0; i < lines.Length; i++)
    //        {
    //            if (lines[i].Contains(tagPastMatchesListOver))
    //            {
    //                break;
    //            }

    //            if (lines[i].Contains(tagDay))
    //            {
    //                dateTime = new SimpleDateTime();

    //                #region month
    //                int monthWordStartIndex = lines[i].IndexOf(tagDay) + tagDay.Length;
    //                int monthWordEndIndex = lines[i].IndexOf(" ", monthWordStartIndex);

    //                char[] monthWord = new char[monthWordEndIndex - monthWordStartIndex];

    //                for (int p = monthWordStartIndex; p < monthWordEndIndex; p++)
    //                {
    //                    monthWord[p - monthWordStartIndex] = lines[i][p];
    //                }

    //                dateTime.Month = monthWordToNumber[new string(monthWord)];
    //                #endregion

    //                #region day
    //                int dayWordStartIndex = monthWordEndIndex + 1;
    //                int dayWordEndIndex = lines[i].IndexOf("th", dayWordStartIndex);
    //                if (dayWordEndIndex == -1)
    //                {
    //                    dayWordEndIndex = lines[i].IndexOf("st", dayWordStartIndex);
    //                }
    //                if (dayWordEndIndex == -1)
    //                {
    //                    dayWordEndIndex = lines[i].IndexOf("nd", dayWordStartIndex);
    //                }
    //                if (dayWordEndIndex == -1)
    //                {
    //                    dayWordEndIndex = lines[i].IndexOf("rd", dayWordStartIndex);
    //                }

    //                char[] dayWord = new char[dayWordEndIndex - dayWordStartIndex];
    //                for (int p = dayWordStartIndex; p < dayWordEndIndex; p++)
    //                {
    //                    dayWord[p - dayWordStartIndex] = lines[i][p];
    //                }

    //                dateTime.Day = int.Parse(new string(dayWord));
    //                #endregion

    //                #region year
    //                int yearWordStartIndex = dayWordEndIndex + 3;
    //                int yearWordEndIndex = yearWordStartIndex + 4;

    //                char[] yearWord = new char[4];
    //                for (int p = yearWordStartIndex; p < yearWordEndIndex; p++)
    //                {
    //                    yearWord[p - yearWordStartIndex] = lines[i][p];
    //                }

    //                dateTime.Year = int.Parse(new string(yearWord));
    //                #endregion
    //            }

    //            if (dateTime != null)
    //            {
    //                if (lines[i].Contains(tagMatchID))
    //                {
    //                    int matchIDStartIndex = lines[i].IndexOf(tagMatchID) + tagMatchID.Length;
    //                    int matchIDEndIndex = lines[i].IndexOf("/", matchIDStartIndex);
    //                    string matchIDString = lines[i].Substring(matchIDStartIndex, matchIDEndIndex - matchIDStartIndex);

    //                    match = new PastMatch { DateTime = dateTime };
    //                    int.TryParse(matchIDString, out match.MatchID);
    //                }

    //                if (lines[i].Contains(tagLoserTeam))
    //                {
    //                    int loserTeamStartIndex = lines[i].IndexOf(tagLoserTeam) + tagLoserTeam.Length;
    //                    int loserTeamEndIndex = lines[i].IndexOf("<", loserTeamStartIndex);
    //                    match.TeamLoser = lines[i].Substring(loserTeamStartIndex, loserTeamEndIndex - loserTeamStartIndex);
    //                }

    //                if (lines[i].Contains(tagWinnerTeam))
    //                {
    //                    int winnerTeamStartIndex = lines[i].IndexOf(tagWinnerTeam) + tagWinnerTeam.Length;
    //                    int winnerTeamEndIndex = lines[i].IndexOf("<", winnerTeamStartIndex);
    //                    match.TeamWinner = lines[i].Substring(winnerTeamStartIndex, winnerTeamEndIndex - winnerTeamStartIndex);
    //                }

    //                if (lines[i].Contains(tagEventName))
    //                {
    //                    int eventNameStartIndex = lines[i].IndexOf(tagEventName) + tagEventName.Length;
    //                    int eventNameEndIndex = lines[i].IndexOf("\"", eventNameStartIndex);
    //                    match.EventName = lines[i].Substring(eventNameStartIndex, eventNameEndIndex - eventNameStartIndex);
    //                }

    //                if (lines[i].Contains(tagMatchFormat))
    //                {
    //                    // если матч бо1, на сайте указывается название карты, в остальных случаях указывается формат(bo3, bo5, etc.)
    //                    int matchFormatStartIndex = lines[i].IndexOf(tagMatchFormat) + tagMatchFormat.Length;
    //                    if (lines[i].Substring(matchFormatStartIndex, 2) == "bo")
    //                    {
    //                        match.Format = int.Parse(lines[i].Substring(matchFormatStartIndex + 2, 1));
    //                    }
    //                    else
    //                    {
    //                        match.Format = 1;
    //                    }
    //                    pastMatches.Add(match);
    //                    match = null;
    //                }
    //            }
    //        }
    //    }
    //    catch
    //    {

    //    }

    //    for (int i = 0; i < pastMatches.Count; i++)
    //    {
    //        string pastMatchURL = string.Empty;
    //        string matchPageHTML = string.Empty;

    //        try
    //        {
    //            pastMatchURL = pastMatches[i].BuildURLToMatch();
    //        }
    //        catch
    //        {
    //            continue;
    //        }

    //        while (!HTMLUtility.GetResponse(pastMatchURL, out matchPageHTML))
    //        {
    //            yield return null;
    //        }
    //        yield return DETAILER_PAST_MATCH(pastMatches[i], matchPageHTML);

    //        try
    //        {
    //            if (pastMatches[i].TeamLoserGlobalRating != 0 && pastMatches[i].TeamWinnerGlobalRating != 0)
    //            {
    //                string json = JsonUtility.ToJson(pastMatches[i]);
    //                File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "Past Matches", "Match " + pastMatches[i].MatchID + ".txt"), json);
    //                Debug.Log(pastMatches[i].ToString());
    //            }
    //        }
    //        catch
    //        {
    //            continue;
    //        }
    //    }
    //}

    //public IEnumerator DETAILER_PAST_MATCH(PastMatch match, string html)
    //{
    //    try
    //    {
    //        string[] lines = html.Split('\n');
    //        bool isMapVetoZone = false;

    //        string tagTeamWinnerPick = (match.TeamWinner + " picked ");
    //        string tagTeamLoserPick = (match.TeamLoser + " picked ");
    //        string tagLeftOverMap = "<div>7. ";

    //        for (int i = 0; i < lines.Length; i++)
    //        {
    //            if (!isMapVetoZone && lines[i].Contains(tagMapVetoStart))
    //            {
    //                isMapVetoZone = true;
    //            }
    //            else if (isMapVetoZone)
    //            {
    //                if (lines[i].Contains(tagLeftOverMap))
    //                {
    //                    int leftOverMapStartIndex = lines[i].IndexOf(tagLeftOverMap) + tagLeftOverMap.Length;
    //                    int leftOverMapEndIndex = lines[i].IndexOf(" was", leftOverMapStartIndex);
    //                    string leftOverMapString = lines[i].Substring(leftOverMapStartIndex, leftOverMapEndIndex - leftOverMapStartIndex);

    //                    match.LeftOverMap = MapsUtility.StringToEMap(leftOverMapString);
    //                    break;
    //                }

    //                if (lines[i].Contains(tagTeamWinnerPick))
    //                {
    //                    int teamWinnerPickStartIndex = lines[i].IndexOf(tagTeamWinnerPick) + tagTeamWinnerPick.Length;
    //                    int teamWinnerPickEndIndex = lines[i].IndexOf("<", teamWinnerPickStartIndex);
    //                    string teamWinnerPickString = lines[i].Substring(teamWinnerPickStartIndex, teamWinnerPickEndIndex - teamWinnerPickStartIndex);

    //                    match.TeamWinnerPick.Add(MapsUtility.StringToEMap(teamWinnerPickString));
    //                }

    //                if (lines[i].Contains(tagTeamLoserPick))
    //                {
    //                    int teamLoserPickStartIndex = lines[i].IndexOf(tagTeamLoserPick) + tagTeamLoserPick.Length;
    //                    int teamLoserPickEndIndex = lines[i].IndexOf("<", teamLoserPickStartIndex);
    //                    string teamLoserPickString = lines[i].Substring(teamLoserPickStartIndex, teamLoserPickEndIndex - teamLoserPickStartIndex);

    //                    match.TeamLoserPick.Add(MapsUtility.StringToEMap(teamLoserPickString));
    //                }
    //            }
    //        }

    //        match.PastMapsID = GET_PAST_MAPS_ID(html);
    //    }
    //    catch
    //    {
    //        yield break;
    //    }

    //    for (int i = 0; i < match.PastMapsID.Count; i++)
    //    {
    //        string urlToPastMap = string.Empty;
    //        string pastMapHTML = string.Empty;

    //        try
    //        {
    //            urlToPastMap = BUILD_URL_TO_PAST_MAPS(match.PastMapsID[i].ID, match.TeamWinner, match.TeamLoser);
    //        }
    //        catch
    //        {
    //            yield break;
    //        }

    //        while (!HTMLUtility.GetResponse(urlToPastMap, out pastMapHTML))
    //        {
    //            yield return null;
    //        }

    //        try
    //        {
    //            PistolRoundsResults pistolRoundsResults = new PistolRoundsResults();
    //            pistolRoundsResults.Map = match.PastMapsID[i].Map;
    //            Dictionary<string, int> pistolRoundsScore = MAP_PISTOL_WINNERS(pastMapHTML);
    //            foreach (string team in pistolRoundsScore.Keys)
    //            {
    //                bool isTeamWinner = (team == match.TeamWinner);

    //                if (isTeamWinner)
    //                {
    //                    pistolRoundsResults.TeamWinnerScore = pistolRoundsScore[team];
    //                }
    //                else
    //                {
    //                    pistolRoundsResults.TeamLoserScore = pistolRoundsScore[team];
    //                }
    //            }

    //            match.PistolRoundsResults.Add(pistolRoundsResults);
    //        }
    //        catch
    //        {
    //            yield break;
    //        }
    //    }

    //    Dictionary<int, List<int>> lineups = new Dictionary<int, List<int>>();

    //    try
    //    {
    //        lineups = PlayersIDHAndler.GetTeampPlayersIDFromMatchPage(html);
    //    }
    //    catch
    //    {
    //        yield break;
    //    }

    //    foreach (int teamID in lineups.Keys)
    //    {
    //        TeamData teamData = null;
    //        bool isTeamWinner = false;

    //        try
    //        {
    //            teamData = TeamIDUtility.GetTeamData(teamID);
    //            isTeamWinner = (teamData.Name == match.TeamWinner);
    //        }
    //        catch
    //        {
    //            yield break;
    //        }

    //        for (int i = 0; i < match.PastMapsID.Count; i++)
    //        {
    //            string url = string.Empty;
    //            string statsPageHTML = string.Empty;

    //            try
    //            {
    //                url = HLTVAPI.BuildURL(match.PastMapsID[i].Map, lineups[teamID].ToArray(), match.DateTime.Subtract(90), match.DateTime);
    //            }
    //            catch
    //            {
    //                yield break;
    //            }

    //            while (!HTMLUtility.GetResponse(url, out statsPageHTML))
    //            {
    //                yield return null;
    //            }

    //            try
    //            {
    //                PistolRoundsStatistics pistolRoundStats = HLTVParcer.GetPistolRoundStats(statsPageHTML);
    //                pistolRoundStats.Map = match.PastMapsID[i].Map;
    //                MapStatistics mapStats = HLTVParcer.GetMapStats(statsPageHTML);
    //                mapStats.Map = match.PastMapsID[i].Map;

    //                if (isTeamWinner)
    //                {
    //                    match.TeamWinnerPistolRoundStats.Add(pistolRoundStats);
    //                    match.TeamWinnerMapStats.Add(mapStats);
    //                }
    //                else
    //                {
    //                    match.TeamLoserPistolRoundStats.Add(pistolRoundStats);
    //                    match.TeamLoserMapStats.Add(mapStats);
    //                }
    //            }
    //            catch
    //            {
    //                yield break;
    //            }
    //        }

    //        int deltaDays = 0;
    //        string teamPageURL = string.Empty;
    //        string teamPageHTML = string.Empty;

    //        try
    //        {
    //            deltaDays = (match.DateTime.DayOfWeek() - DayOfWeek.Monday);
    //            teamPageURL = TeamIDUtility.BuildURLToTeamOverviewPage(teamData);
    //        }
    //        catch
    //        {
    //            yield break;
    //        }

    //        while (!HTMLUtility.GetResponse(teamPageURL, out teamPageHTML))
    //        {
    //            yield return null;
    //        }

    //        try
    //        {
    //            int rating = GET_RATING(teamPageHTML, match.DateTime.Subtract(deltaDays));

    //            if (isTeamWinner)
    //            {
    //                match.TeamWinnerGlobalRating = rating;
    //            }
    //            else
    //            {
    //                match.TeamLoserGlobalRating = rating;
    //            }
    //        }
    //        catch
    //        {
    //            yield break;
    //        }
    //    }
    //}

    public static List<PastMapID> GET_PAST_MAPS_ID(string html)
    {
        List<PastMapID> pastMapsID = new List<PastMapID>();

        string tagMapsIDStart = "dynamic-map-name-full\" id=\"all\">All maps</div>";
        string tagMapID = "dynamic-map-name-full\" id=\"";

        string[] lines = html.Split('\n');

        bool isMapsIDZone = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (!isMapsIDZone && lines[i].Contains(tagMapsIDStart))
            {
                isMapsIDZone = true;
                continue;
            }
            else if (isMapsIDZone)
            {
                if (lines[i].Contains(tagMapID))
                {
                    int mapIDStartIndex = lines[i].IndexOf(tagMapID) + tagMapID.Length;
                    int mapIDEndIndex = lines[i].IndexOf("\"", mapIDStartIndex);
                    string mapIDString = lines[i].Substring(mapIDStartIndex, mapIDEndIndex - mapIDStartIndex);
                    int mapID = int.Parse(mapIDString);

                    int mapNameStartIndex = mapIDEndIndex + 2;
                    int mapNameEndIndex = lines[i].IndexOf("<", mapNameStartIndex);
                    string mapName = lines[i].Substring(mapNameStartIndex, mapNameEndIndex - mapNameStartIndex);

                    pastMapsID.Add(new PastMapID { Map = MapsUtility.StringToEMap(mapName), ID = mapID });
                }
            }
        }

        return pastMapsID;
    }

    public static string BUILD_URL_TO_PAST_MAPS(int mapID, string teamOne, string teamTwo)
    {
        //https://www.hltv.org/stats/matches/mapstatsid/114543/pain-vs-rebirth

        string url = "https://www.hltv.org/stats/matches/mapstatsid/";
        url += (mapID + "/");
        url += (TeamIDUtility.CorrectTeamNameForURL(teamOne) + "-vs-" + TeamIDUtility.CorrectTeamNameForURL(teamTwo));

        return url;
    }

    public static Dictionary<string, int> MAP_PISTOL_WINNERS(string html)
    {
        Dictionary<string, int> teamsPistorRoundsScore = new Dictionary<string, int>();

        string tagRoundHistoryStart = "Round history</div>";

        string[] lines = html.Split('\n');

        string tagScoreLine = "<span class=\"won\">";
        string tagFirstHalfFirstTeamScoreIfT = "( <span class=\"t-color\">";
        string tagFirstHalfFirstTeamScoreIfCT = "( <span class=\"ct-color\">";
        string tagFirstTeam = "<div class=\"team-left\">";
        string tagSecondTeam = "<div class=\"team-right\">";
        string tagTeamName = "title=\"";

        int firstHalfFirstTeamScore = 0;
        int sixteenthRoundForTeamOne = firstHalfFirstTeamScore + 1;
        int sixteenthRoundForTeamTwo = 16 - firstHalfFirstTeamScore;

        string firstTeamName = string.Empty;
        string secondTeamName = string.Empty;

        bool isRoundsHistoryZone = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(tagFirstTeam))
            {
                int firstTeamNameStartIndex = lines[i].IndexOf(tagTeamName) + tagTeamName.Length;
                int firstTeamNameEndIndex = lines[i].IndexOf("\"", firstTeamNameStartIndex);
                firstTeamName = lines[i].Substring(firstTeamNameStartIndex, firstTeamNameEndIndex - firstTeamNameStartIndex);
                continue;
            }

            if (lines[i].Contains(tagSecondTeam))
            {
                int secondTeamNameStartIndex = lines[i].IndexOf(tagTeamName) + tagTeamName.Length;
                int secondTeamNameEndIndex = lines[i].IndexOf("\"", secondTeamNameStartIndex);
                secondTeamName = lines[i].Substring(secondTeamNameStartIndex, secondTeamNameEndIndex - secondTeamNameStartIndex);
                continue;
            }

            if (lines[i].Contains(tagScoreLine))
            {
                int tScoreIndex = lines[i].IndexOf(tagFirstHalfFirstTeamScoreIfT) + tagFirstHalfFirstTeamScoreIfT.Length;
                int ctScoreIndex = lines[i].IndexOf(tagFirstHalfFirstTeamScoreIfCT) + tagFirstHalfFirstTeamScoreIfCT.Length;

                int firstHalfFirstTeamScoreStartIndex;

                if (tScoreIndex < ctScoreIndex)
                {
                    firstHalfFirstTeamScoreStartIndex = tScoreIndex;
                }
                else
                {
                    firstHalfFirstTeamScoreStartIndex = ctScoreIndex;
                }

                int firstHalfFirstTeamScoreEndIndex = lines[i].IndexOf("<", firstHalfFirstTeamScoreStartIndex);

                string firstHalfFirstTeamScoreString = lines[i].Substring(firstHalfFirstTeamScoreStartIndex, firstHalfFirstTeamScoreEndIndex - firstHalfFirstTeamScoreStartIndex);

                firstHalfFirstTeamScore = int.Parse(firstHalfFirstTeamScoreString);
                sixteenthRoundForTeamOne = firstHalfFirstTeamScore + 1;
                sixteenthRoundForTeamTwo = 16 - firstHalfFirstTeamScore;
                continue;
            }

            if (!isRoundsHistoryZone && lines[i].Contains(tagRoundHistoryStart))
            {
                isRoundsHistoryZone = true;
                continue;
            }
            else if (isRoundsHistoryZone)
            {
                if (lines[i].Contains(firstTeamName))
                {
                    teamsPistorRoundsScore.Add(firstTeamName, 0);

                    string firstHalfMapHTMLLine = lines[i + 1];
                    string secondHalfMapHTMLLine = lines[i + 2];

                    string tagFirstTeamFirstPistolRoundWin = "title=\"1-0\"";
                    string tagFirstTeamSecondPistolRoundWin = ("title=\"" + sixteenthRoundForTeamOne + "-" + (16 - sixteenthRoundForTeamOne) + "\"");

                    if (firstHalfMapHTMLLine.Contains(tagFirstTeamFirstPistolRoundWin))
                    {
                        teamsPistorRoundsScore[firstTeamName]++;
                    }

                    if (secondHalfMapHTMLLine.Contains(tagFirstTeamSecondPistolRoundWin))
                    {
                        teamsPistorRoundsScore[firstTeamName]++;
                    }
                }

                if (lines[i].Contains(secondTeamName))
                {
                    teamsPistorRoundsScore.Add(secondTeamName, 0);

                    string firstHalfMapHTMLLine = lines[i + 1];
                    string secondHalfMapHTMLLine = lines[i + 2];

                    string tagSecondTeamFirstPistolRoundWin = "title=\"0-1\"";
                    string tagSecondTeamSecondPistolRoundWin = ("title=\"" + (16 - sixteenthRoundForTeamTwo) + "-" + sixteenthRoundForTeamTwo + "\"");

                    if (firstHalfMapHTMLLine.Contains(tagSecondTeamFirstPistolRoundWin))
                    {
                        teamsPistorRoundsScore[secondTeamName]++;
                    }

                    if (secondHalfMapHTMLLine.Contains(tagSecondTeamSecondPistolRoundWin))
                    {
                        teamsPistorRoundsScore[secondTeamName]++;
                    }

                    break;
                }
            }
        }

        return teamsPistorRoundsScore;
    }

    public static int GET_RATING(string html, SimpleDateTime dateTime)
    {
        int rating = 0;

        string dateTimeInWords = dateTime.FromNumbersToWords();

        string[] lines = html.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(dateTimeInWords))
            {
                int ratingStartIndex = lines[i].IndexOf("#", lines[i].IndexOf(dateTimeInWords) + dateTimeInWords.Length) + 1;
                int ratingEndIndex = lines[i].IndexOf("\\", ratingStartIndex);
                string ratingString = lines[i].Substring(ratingStartIndex, ratingEndIndex - ratingStartIndex);

                int.TryParse(ratingString, out rating);
                break;
            }
        }

        return rating;
    }
}