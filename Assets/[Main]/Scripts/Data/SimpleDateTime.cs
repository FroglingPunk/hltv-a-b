using System;

[Serializable]
public class SimpleDateTime
{
    public int Year;
    public int Month;
    public int Day;


    public SimpleDateTime(int year, int month, int day)
    {
        Year = year;
        Month = month;
        Day = day;
    }

    public SimpleDateTime(SimpleDateTime simpleDateTime)
    {
        Year = simpleDateTime.Year;
        Month = simpleDateTime.Month;
        Day = simpleDateTime.Day;
    }

    public SimpleDateTime() { }

    public SimpleDateTime(DateTime dateTime)
    {
        Year = dateTime.Year;
        Month = dateTime.Month;
        Day = dateTime.Day;
    }

    public DayOfWeek DayOfWeek()
    {
        return new DateTime(Year, Month, Day).DayOfWeek;
    }

    public SimpleDateTime Subtract(int days)
    {
        DateTime dateTime = new DateTime(Year, Month, Day).Subtract(new TimeSpan(days, 0, 0, 0, 0));
        return new SimpleDateTime(dateTime);
    }

    public static SimpleDateTime Now
    {
        get
        {
            DateTime dateTimeNow = DateTime.Now;
            return new SimpleDateTime(dateTimeNow);
        }
    }


    public override string ToString()
    {
        return Day + "." + Month + "." + Year;
    }
}