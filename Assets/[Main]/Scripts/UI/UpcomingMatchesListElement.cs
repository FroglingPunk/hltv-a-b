using UnityEngine;
using UnityEngine.UI;

public class UpcomingMatchesListElement : MonoBehaviour
{
    [SerializeField] private Text textFirstTeamName;
    [SerializeField] private Text textSecondTeamName;
    [SerializeField] private Text textDateTime;
    [SerializeField] private Text textEventName;


    public void Init(UpcomingMatch upcomingMatch)
    {
        textFirstTeamName.text = upcomingMatch.FirstTeamID.ToString();
        textSecondTeamName.text = upcomingMatch.SecondTeamID.ToString();
        textDateTime.text = upcomingMatch.DateTime.ToString();
        textEventName.text = upcomingMatch.EventName;
    }
}