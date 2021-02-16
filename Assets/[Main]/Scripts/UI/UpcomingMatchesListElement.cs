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
        textFirstTeamName.text = TeamIDUtility.GetTeamData(upcomingMatch.FirstTeamID).Name;
        textSecondTeamName.text = TeamIDUtility.GetTeamData(upcomingMatch.SecondTeamID).Name;
        textDateTime.text = upcomingMatch.DateTime.ToString();
        textEventName.text = upcomingMatch.EventName;
    }
}