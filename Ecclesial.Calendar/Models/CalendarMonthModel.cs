using System.Collections.Generic;

namespace Ecclesial.Calendar.Models
{
    public class CalendarMonthModel
    {
        public int Month { get; set; }
        public string Name { get; set; }
        public IList<CalendarWeekModel> Weeks { get; set; } = new List<CalendarWeekModel>();
    }
}