using System;
using System.Collections.Generic;
using System.Text;

namespace Fosol.Core.Extensions.DateTimes
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, date.Kind);
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0, date.Kind);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, date.Kind);
        }

        public static DateTime StartOfWeek(this DateTime date)
        {
            return new DateTime(date.Ticks, date.Kind).AddDays(-1 * (int)date.DayOfWeek);
        }
        
        public static DateTime SetTime(this DateTime date, int hour, int minute = 0, int second = 0, int millisecond = 0)
        {
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second, millisecond, date.Kind);
        }
    }
}
