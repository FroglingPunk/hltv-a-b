using System;

[Serializable]
public class UpcomingMatch
{
    public int FirstTeamID;
    public int SecondTeamID;
    public int Format;

    public string EventName;
    public int EventID;

    public DateTime DateTime;


    public override string ToString()
    {
        return FirstTeamID + " VS " + SecondTeamID + " BO" + Format + " on " + DateTime.ToString();
    }
}