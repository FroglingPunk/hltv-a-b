using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Analyzer : MonoBehaviour
{
    public static string PastMatchesFolderPath => Path.Combine(Application.dataPath.Replace("Assets", ""), "Parsing Results", "DEBUG");


    void Start()
    {
        TOTALWORK();
    }


    private void TOTALWORK()
    {
        PastMatch[] pastMatches = LoadPastMatchFromJson();

        //CheckForDefects(pastMatches);

        PastMatch match = null;

        int counterLoseBothPistolsWithBetterStats = 0;
        int counterWinOneOrMOrePistolsWithBetterStats = 0;
        int counterErrors = 0;

        for (int i = 0; i < pastMatches.Length; i++)
        {
            match = pastMatches[i];

            if (match.Format < 2)
            {
                continue;
            }

            //if (match.TeamWinnerGlobalRating < 20 || match.TeamWinnerGlobalRating > 200 ||
            //    match.TeamLoserGlobalRating < 20 || match.TeamLoserGlobalRating > 200)
            //{
            //    continue;
            //}
           
            for (int p = 0; p < match.PistolRoundsResults.Count; p++)
            {
                try
                {
                    PistolRoundsResults result = match.PistolRoundsResults[p];
                    PistolRoundsStatistics teamWinnerPistolStats = null;
                    PistolRoundsStatistics teamLoserPistolStats = null;

                    // в случае дисквалификации или автопоражения указывается лишь 1 карта, но формат остаётся БО3
                    if (match.PastMapsID.Count < 2)
                    {
                        continue;
                    }

                    if (match.PastMapsID[1].Map != match.PistolRoundsResults[p].Map)
                    {
                        continue;
                    }

                    for (int u = 0; u < match.TeamWinnerPistolRoundStats.Count; u++)
                    {
                        if (match.TeamWinnerPistolRoundStats[u].Map == result.Map)
                        {
                            teamWinnerPistolStats = match.TeamWinnerPistolRoundStats[u];
                            break;
                        }
                    }

                    for (int u = 0; u < match.TeamLoserPistolRoundStats.Count; u++)
                    {
                        if (match.TeamLoserPistolRoundStats[u].Map == result.Map)
                        {
                            teamLoserPistolStats = match.TeamLoserPistolRoundStats[u];
                            break;
                        }
                    }

                    if (teamWinnerPistolStats.PistolRounds < 4 || teamLoserPistolStats.PistolRounds < 4)
                    {
                        continue;
                    }

                    if (!match.TeamWinnerPick.Contains(result.Map) && !match.TeamLoserPick.Contains(result.Map))
                    {
                        continue;
                    }

                    //double delta = teamWinnerPistolStats.TotalPistolRoundWinPercent - teamLoserPistolStats.TotalPistolRoundWinPercent;

                    if (match.TeamWinnerPick.Contains(result.Map) &&
                        teamWinnerPistolStats.TotalPistolRoundWinPercent > 70 && teamLoserPistolStats.TotalPistolRoundWinPercent < 40)
                    //&&                    match.TeamWinnerGlobalRating - match.TeamLoserGlobalRating > 10)
                    {
                        if (result.TeamWinnerScore == 0)
                        {
                            counterLoseBothPistolsWithBetterStats++;
                        }
                        else
                        {
                            counterWinOneOrMOrePistolsWithBetterStats++;
                        }
                    }

                    if (match.TeamLoserPick.Contains(result.Map) &&
                        teamLoserPistolStats.TotalPistolRoundWinPercent > 70 && teamWinnerPistolStats.TotalPistolRoundWinPercent < 40)
                    //&&                    match.TeamLoserGlobalRating - match.TeamWinnerGlobalRating > 10)
                    {
                        if (result.TeamLoserScore == 0)
                        {
                            counterLoseBothPistolsWithBetterStats++;
                        }
                        else
                        {
                            counterWinOneOrMOrePistolsWithBetterStats++;
                        }
                    }
                }
                catch
                {
                    counterErrors++;
                    continue;
                }
            }
        }

        Debug.Log("MATCHES ANALYZED ::: " + pastMatches.Length);
        Debug.Log("ERRORS ::: " + counterErrors);
        Debug.Log("LOSE BOTH PISTOLS WITH BETTER STATS ::: " + counterLoseBothPistolsWithBetterStats);
        Debug.Log("WIN ONE OR MORE PISTOLS WITH BETTER STATS ::: " + counterWinOneOrMOrePistolsWithBetterStats);
    }

    private PastMatch[] LoadPastMatchFromJson()
    {
        string[] jsonFilesPath = Directory.GetFiles(PastMatchesFolderPath);

        PastMatch[] pastMatches = new PastMatch[jsonFilesPath.Length];

        for (int i = 0; i < jsonFilesPath.Length; i++)
        {
            string json = File.ReadAllText(jsonFilesPath[i]);
            pastMatches[i] = JsonUtility.FromJson<PastMatch>(json);
        }

        return pastMatches;
    }

    private void CheckForDefects(PastMatch[] pastMatches)
    {
        PastMatch match = null;

        int counterWithoutRating = 0;
        int counterWithoutPistolRoundsCalc = 0;

        for (int i = 0; i < pastMatches.Length; i++)
        {
            match = pastMatches[i];

            if (match.TeamLoserGlobalRating == 0 || match.TeamWinnerGlobalRating == 0)
            {
                counterWithoutRating++;
            }

            for (int p = 0; p < match.PistolRoundsResults.Count; p++)
            {
                if (match.PistolRoundsResults[p].TeamLoserScore == 0 && match.PistolRoundsResults[p].TeamWinnerScore == 0)
                {
                    counterWithoutPistolRoundsCalc++;
                }
            }
        }

        Debug.Log("WITHOUT RATING ::: " + counterWithoutRating);
        Debug.Log("WITHOUT PISTOL ROUNDS CALC ::: " + counterWithoutPistolRoundsCalc);
    }

    private void CheckForMartingale(PastMatch[] pastMatches)
    {
        //недописан

        PastMatch match = null;

        int counterFormatMoreOne = 0;
        int counterZeroPistolWins = 0;

        for (int i = 0; i < pastMatches.Length; i++)
        {
            match = pastMatches[i];

            if (match.Format < 2)
            {
                continue;
            }

            counterFormatMoreOne++;

            bool teamWinnerhasWins = false;
            bool teamLoserhasWins = false;

            for (int p = 0; p < match.PistolRoundsResults.Count; p++)
            {
                if (match.PistolRoundsResults[p].TeamLoserScore > 0)
                {
                    teamLoserhasWins = true;
                }

                if (match.PistolRoundsResults[p].TeamWinnerScore > 0)
                {
                    teamWinnerhasWins = true;
                }
            }

            if (!teamWinnerhasWins && !teamLoserhasWins)
            {
                counterZeroPistolWins++;
            }
        }

        Debug.Log("FORMAT MORE ONE MAP ::: " + counterFormatMoreOne);
        Debug.Log("WITHOUT PISTOL ROUNDS CALC ::: " + counterZeroPistolWins);
    }
}