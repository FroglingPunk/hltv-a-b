using System;

[Serializable]
public class TeamStatistics
{
    public int ID;

    public string Team;

    public MapStatistics[] mapsStatistics;


    public MapStatistics GetMapStatistics(string map)
    {
        for (int i = 0; i < mapsStatistics.Length; i++)
        {
            if (mapsStatistics[i].Map == map)
            {
                return mapsStatistics[i];
            }
        }

        return null;
    }


    public override string ToString()
    {
        return Team + "\nID : " + ID;
    }
}