using System.Collections.Generic;
using UnityEngine;

public static class GameTime 
{
    public static float daysTotal;

    public static float days;

    public static float monthsTotal;

    public static int month;

    public static List<string>
        monthNames;

    public static string currentMonth;

    public static float daysInMonth;

    public static float year;

    public static string Date {get; set;}

    public static void HandleTime()
    {
        if (Is30DayMonth())
        {
            daysInMonth = 30;
        }
        else if (Is31DayMonth())
        {
            daysInMonth = 31;
        }
        else
        {
            daysInMonth = 28;
        }

        if (days == daysInMonth)
        {
            month++;
            days = 1;
            currentMonth = monthNames[month];
        }

        if (month == monthsTotal)
        {
            year++;
            month = 0;
            currentMonth = monthNames[month];
        }

        Date = ParseDate();

        
    }

    public static bool Is31DayMonth()
    {
        if (month is 0 or 2 or 4 or 6 or 7 or 9 or 11)
        {
            return true;
        }
        return false;
    }

    public static bool Is30DayMonth()
    {
        if (month is 3 or 5 or 8 or 10)
        {
            return true;
        }
        return false;
    }

    public static string ParseDate(){
        Date = currentMonth + " " + days + ", " + year;
        return Date;
    }

    public static string GetDate()
    {
        return Date;
    }
}
