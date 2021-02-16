using UnityEngine;
using UnityEngine.UI;

public class Starter : MonoBehaviour
{
    [SerializeField] private PastMatchesAPI pastMatchesAPI;

    [SerializeField] private Button buttonStart;

    [SerializeField] private InputField inputFieldStartOffset;
    [SerializeField] private InputField inputFieldCount;

    [SerializeField] private InputField inputFieldStartDateYear;
    [SerializeField] private InputField inputFieldStartDateMonth;
    [SerializeField] private InputField inputFieldStartDateDay;

    [SerializeField] private InputField inputFieldEndDateYear;
    [SerializeField] private InputField inputFieldEndDateMonth;
    [SerializeField] private InputField inputFieldEndDateDay;


    void Awake()
    {
        buttonStart.onClick.AddListener(OnButtonStartPressed);

        inputFieldStartOffset.text = pastMatchesAPI.startOffset.ToString();
        inputFieldStartOffset.onValueChanged.AddListener((value) =>
        {
            if(value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.startOffset);
            }
        });

        inputFieldCount.text = pastMatchesAPI.matchesCount.ToString();
        inputFieldCount.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.matchesCount);
            }
        });

        inputFieldStartDateYear.text = pastMatchesAPI.totalWorkStartTime.Year.ToString();
        inputFieldStartDateYear.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.totalWorkStartTime.Year);
            }
        });

        inputFieldStartDateMonth.text = pastMatchesAPI.totalWorkStartTime.Month.ToString();
        inputFieldStartDateMonth.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.totalWorkStartTime.Month);
            }
        });

        inputFieldStartDateDay.text = pastMatchesAPI.totalWorkStartTime.Day.ToString();
        inputFieldStartDateDay.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.totalWorkStartTime.Day);
            }
        });

        inputFieldEndDateYear.text = pastMatchesAPI.totalWorkEndTime.Year.ToString();
        inputFieldEndDateYear.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.totalWorkEndTime.Year);
            }
        });

        inputFieldEndDateMonth.text = pastMatchesAPI.totalWorkEndTime.Month.ToString();
        inputFieldEndDateMonth.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.totalWorkEndTime.Month);
            }
        });

        inputFieldEndDateDay.text = pastMatchesAPI.totalWorkEndTime.Day.ToString();
        inputFieldEndDateDay.onValueChanged.AddListener((value) =>
        {
            if (value != string.Empty)
            {
                int.TryParse(value, out pastMatchesAPI.totalWorkEndTime.Day);
            }
        });
    }

    void OnDestroy()
    {
        buttonStart.onClick.RemoveAllListeners();

        inputFieldStartOffset.onValueChanged.RemoveAllListeners();
        inputFieldCount.onValueChanged.RemoveAllListeners();

        inputFieldStartDateYear.onValueChanged.RemoveAllListeners();
        inputFieldStartDateMonth.onValueChanged.RemoveAllListeners();
        inputFieldStartDateDay.onValueChanged.RemoveAllListeners();

        inputFieldEndDateYear.onValueChanged.RemoveAllListeners();
        inputFieldEndDateMonth.onValueChanged.RemoveAllListeners();
        inputFieldEndDateDay.onValueChanged.RemoveAllListeners();
    }


    private void OnButtonStartPressed()
    {
        if (!pastMatchesAPI.ParserIsWork)
        {
            pastMatchesAPI.StartParcer();
        }
    }
}