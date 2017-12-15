using System;
using System.Collections.Generic;

namespace Ecclesial.Calendar.Models
{
    public class CalendarEventModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }
        public IList<CalendarTaskModel> Tasks { get; set; } = new List<CalendarTaskModel>();
    }
}