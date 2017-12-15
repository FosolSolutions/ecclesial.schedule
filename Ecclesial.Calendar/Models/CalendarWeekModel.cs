using System.Collections.Generic;

namespace Ecclesial.Calendar.Models
{
    public class CalendarWeekModel
    {
        public int Day { get; set; }
        public string Name { get; set; }
        public IList<CalendarEventModel> Events { get; set; } = new List<CalendarEventModel>();
    }
}