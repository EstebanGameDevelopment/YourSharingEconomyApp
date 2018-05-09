using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ExtentionFlatCalendar
{
    public static void SetMyTime(this FlatCalendar _calendar, System.DateTime _date)
    {
        _calendar.currentTime.year = _date.Year;
        _calendar.currentTime.month = _date.Month;
        _calendar.currentTime.day = _date.Day;
        _calendar.currentTime.dayOfWeek = _date.DayOfWeek.ToString();
        _calendar.currentTime.totalDays = System.DateTime.DaysInMonth(_calendar.currentTime.year, _calendar.currentTime.month);
        _calendar.currentTime.dayOffset = GetIndexOfFirstSlotInMonth(_calendar.currentTime.year, _calendar.currentTime.month);
        _calendar.updateCalendar(_calendar.currentTime.month, _calendar.currentTime.year);
        _calendar.markSelectionDay(_calendar.currentTime.day);
    }

    public static int GetIndexOfFirstSlotInMonth(int year, int month)
    {
        int indexOfFirstSlot = 0;

        System.DateTime dateValue = new System.DateTime(year, month, 1);
        string dayOfWeek = dateValue.DayOfWeek.ToString();

        if (dayOfWeek == "Monday") indexOfFirstSlot = 0;
        if (dayOfWeek == "Tuesday") indexOfFirstSlot = 1;
        if (dayOfWeek == "Wednesday") indexOfFirstSlot = 2;
        if (dayOfWeek == "Thursday") indexOfFirstSlot = 3;
        if (dayOfWeek == "Friday") indexOfFirstSlot = 4;
        if (dayOfWeek == "Saturday") indexOfFirstSlot = 5;
        if (dayOfWeek == "Sunday") indexOfFirstSlot = 6;

        return indexOfFirstSlot;
    }
}
