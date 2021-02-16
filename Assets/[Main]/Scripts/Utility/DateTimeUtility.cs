using System.Collections.Generic;

public static class DateTimeUtility
{
    private static Dictionary<int, string> numberToMonthWord = new Dictionary<int, string>
    {
        { 1, "January" },
        { 2, "February" },
        { 3, "March" },
        { 4, "April" },
        { 5, "May" },
        { 6, "June" },
        { 7, "July" },
        { 8, "August" },
        { 9, "September" },
        { 10, "October" },
        { 11, "November" },
        { 12, "December" }
    };


    public static string CorrectForURL(this SimpleDateTime dateTime)
    {
        string urlDateTime = string.Empty;
        urlDateTime += (dateTime.Year + "-");

        if (dateTime.Month < 10)
        {
            urlDateTime += ("0" + dateTime.Month + "-");
        }
        else
        {
            urlDateTime += (dateTime.Month + "-");
        }

        if (dateTime.Day < 10)
        {
            urlDateTime += ("0" + dateTime.Day);
        }
        else
        {
            urlDateTime += (dateTime.Day);
        }

        return urlDateTime;
    }

    public static string FromNumbersToWords(this SimpleDateTime dateTime)
    {
        string words = dateTime.Day.ToString();
        if(dateTime.Day == 1)
        {
            words += "st";
        }
        else if(dateTime.Day == 2)
        {
            words += "nd";
        }
        else if(dateTime.Day == 3)
        {
            words += "rd";
        }
        else
        {
            words += "th";
        }

        words += " " + numberToMonthWord[dateTime.Month];
        words += " " + dateTime.Year;
        return words;
    }
}