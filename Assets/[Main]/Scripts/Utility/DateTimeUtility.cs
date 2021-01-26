public static class DateTimeUtility
{
    public static string CorrectForURL(this System.DateTime dateTime)
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
            urlDateTime += ("0" + dateTime.Day + "-");
        }
        else
        {
            urlDateTime += (dateTime.Day);
        }

        return urlDateTime;
    }
}