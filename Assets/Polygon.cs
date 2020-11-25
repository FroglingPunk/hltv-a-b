using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class Polygon : MonoBehaviour
{
    public bool GET_RESPONSE = false;
    public bool DB_UNPACK = false;
    public string uri;

    void OnValidate()
    {
        if (GET_RESPONSE)
        {
            GET_RESPONSE = false;
            string html = getResponse(uri);

            if (!Directory.Exists(Application.streamingAssetsPath)) { Directory.CreateDirectory(Application.streamingAssetsPath); }
            using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "html.txt")))
            {
                stream.Write(html);
            }

            List<TeamStatistics> allTeams = GetAllTeams(html);

            //List<TeamStatistics> allTeams = new List<TeamStatistics>()
            //{
            //    new TeamStatistics{ Team = "g2", ID = 5995}
            //};

            for (int i = 0; i < allTeams.Count; i++)
            {
                Debug.Log(allTeams[i].ToString());

                string htmlPath = @"https://www.hltv.org/team/" + allTeams[i].ID + @"/" + allTeams[i].Team + @"#tab-statsBox";
                allTeams[i].mapsStatistics = GetPistolRoundStats(getResponse(htmlPath)).ToArray();
            }

            using (StreamWriter stream = File.CreateText(Path.Combine(Application.streamingAssetsPath, "db.txt")))
            {
                for (int i = 0; i < allTeams.Count; i++)
                {
                    string json = JsonUtility.ToJson(allTeams[i]);
                    stream.WriteLine(json);
                }
            }

            //List<MapStatistics> mapsStatistics = GetPistolRoundStats(html);

            //for (int i = 0; i < mapsStatistics.Count; i++)
            //{
            //    Debug.Log(mapsStatistics[i].ToString());
            //}
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
                    for(int p = 0; p < teamStatistics[i].mapsStatistics.Length; p++)
                    {
                        Debug.Log(teamStatistics[i].mapsStatistics[p]);
                    }
                }
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

    [System.Serializable]
    public class MapStatistics
    {
        public string Map;
        public float PistolRoundWin;


        public override string ToString()
        {
            return Map + "\nPistol Round Win Rate : " + PistolRoundWin.ToString();
        }
    }
    [System.Serializable]
    public class TeamStatistics
    {
        public int ID;
        public string Team;
        public MapStatistics[] mapsStatistics;


        public override string ToString()
        {
            return Team + "\nID : " + ID;
        }
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

            mapStats.PistolRoundWin = float.Parse(PRstring);

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

                for(int p = index + dbg.Length; p < finishIndex; p++)
                {
                    temp[p - index - dbg.Length] = strings[i][p];
                }

                string teamID = new string(temp);/*new char[]*/
                //{
                //    strings[i][index + dbg.Length],
                //    strings[i][index + dbg.Length + 1],
                //    strings[i][index + dbg.Length + 2],
                //    strings[i][index + dbg.Length + 3]
                //});

                int.TryParse(teamID, out teamStats.ID);

                if( strings[i].IndexOf("data-tooltip-id") > 0)
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

    static string getResponse(string uri)
    {
        StringBuilder sb = new StringBuilder();
        byte[] buf = new byte[8192];
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream resStream = response.GetResponseStream();
        int count = 0;
        do
        {
            count = resStream.Read(buf, 0, buf.Length);
            if (count != 0)
            {
                sb.Append(Encoding.Default.GetString(buf, 0, count));
            }
        }
        while (count > 0);
        return sb.ToString();
    }
}