using System.Collections.Generic;

public enum EMap
{
    Dust2,
    Mirage,
    Inferno,
    Nuke,
    Train,
    Overpass,
    Vertigo
}

public static class MapsUtility
{
    private static Dictionary<EMap, int> mapsID = new Dictionary<EMap, int>
    {
        { EMap.Dust2, 31},
        { EMap.Mirage, 32 },
        { EMap.Inferno, 33 },
        { EMap.Nuke, 34 },
        { EMap.Train, 35 },
        { EMap.Overpass, 40 },
        { EMap.Vertigo, 46 }
    };

    private static Dictionary<string, EMap> mapsStrings = new Dictionary<string, EMap>
    {
        { "Dust2", EMap.Dust2},
        { "Mirage", EMap.Mirage },
        { "Inferno", EMap.Inferno },
        { "Nuke", EMap.Nuke },
        { "Train", EMap.Train },
        { "Overpass", EMap.Overpass },
        { "Vertigo", EMap.Vertigo }
    };


    public static int GetMapID(this EMap map)
    {
        return mapsID.ContainsKey(map) ? mapsID[map] : 0;
    }

    public static EMap StringToEMap(string mapString)
    {
        try
        {
            return mapsStrings[mapString];
        }
        catch
        {
            UnityEngine.Debug.LogError("Попытка преобразовать некорректный текст в EMap");
            return EMap.Vertigo;
        }
    }
}