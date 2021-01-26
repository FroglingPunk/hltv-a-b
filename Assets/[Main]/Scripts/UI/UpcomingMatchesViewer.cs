using System.Collections.Generic;
using UnityEngine;

public class UpcomingMatchesViewer : MonoBehaviour
{
    [SerializeField] private UpcomingMatchesListElement elementPrefab;
    [SerializeField] private RectTransform content;


    void Start()
    {
        List<UpcomingMatch> upcomingMatches = UpcomingMatchesHandler.GetUpcomingMatches();

        for(int i = 0; i < upcomingMatches.Count; i++)
        {
            UpcomingMatchesListElement element = Instantiate(elementPrefab, content);
            element.Init(upcomingMatches[i]);
        }
    }
}