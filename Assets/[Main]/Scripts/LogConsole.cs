using UnityEngine;
using UnityEngine.UI;

public class LogConsole : MonoBehaviour
{
    [SerializeField] private PastMatchesAPI pastMatchesAPI;
    [SerializeField] private RectTransform content;
    [SerializeField] private Text textObjectPrefab;


    void Awake()
    {
        pastMatchesAPI.OnParserStart += OnParserStart;
        pastMatchesAPI.OnParserFinish += OnParserFinish;
        pastMatchesAPI.OnMatchParsingResultRecorded += OnMatchParsingResultRecorded;
        pastMatchesAPI.OnMatchParsingError += OnMatchParsingError;
    }

    void OnDestroy()
    {
        pastMatchesAPI.OnParserStart -= OnParserStart;
        pastMatchesAPI.OnParserFinish -= OnParserFinish;
        pastMatchesAPI.OnMatchParsingResultRecorded -= OnMatchParsingResultRecorded;
        pastMatchesAPI.OnMatchParsingError -= OnMatchParsingError;
    }


    private void OnParserStart()
    {
        Text textObject = Instantiate(textObjectPrefab, content);
        textObject.name = "text_parserStart";
        textObject.text = "!!! PARSER START !!!";
        textObject.color = Color.blue;
        textObject.transform.SetParent(content);
    }

    private void OnParserFinish()
    {
        Text textObject = Instantiate(textObjectPrefab, content);
        textObject.name = "text_parserFinish";
        textObject.text = "!!! PARSER FINISH !!!";
        textObject.color = Color.blue;
        textObject.transform.SetParent(content);
    }

    private void OnMatchParsingResultRecorded(PastMatch match)
    {
        Text textObject = Instantiate(textObjectPrefab, content);
        textObject.name = "text_parserMatchRecorded";
        textObject.text = (match.TeamWinner + " VS " + match.TeamLoser + " " + match.DateTime);
        textObject.color = Color.green;
        textObject.transform.SetParent(content);
    }

    private void OnMatchParsingError(PastMatch match)
    {
        Text textObject = Instantiate(textObjectPrefab, content);
        textObject.name = "text_parserError";
        textObject.text = ("ERROR WITH LINEUP PARSING ::: " + match.TeamWinner + " VS " + match.TeamLoser + " " + match.DateTime);
        textObject.color = Color.red;
        textObject.transform.SetParent(content);
    }
}