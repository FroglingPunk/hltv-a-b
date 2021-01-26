using System.Collections.Generic;

public static class PlayersIDHAndler
{
    private static string tagPlayerID = "a href=\"/player/";


    public static List<int> GetTeampPlayersID(string teamPageHTML)
    {
        List<int> playersID = new List<int>();

        string[] strings = teamPageHTML.Split('\n');

        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains(tagPlayerID))
            {
                int startIndex = strings[i].IndexOf(tagPlayerID) + tagPlayerID.Length;
                int endIndex = strings[i].IndexOf('/', startIndex);

                char[] charsID = new char[endIndex - startIndex];

                for (int p = startIndex; p < endIndex; p++)
                {
                    charsID[p - startIndex] = strings[i][p];
                }

                int currentPlayerID = -1;
                int.TryParse(new string(charsID), out currentPlayerID);
                playersID.Add(currentPlayerID);
            }
        }

        return playersID;
    }
}