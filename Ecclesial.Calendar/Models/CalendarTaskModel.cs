using System.Collections.Generic;

namespace Ecclesial.Calendar.Models
{
    public class CalendarTaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public byte[] RowVersion { get; set; }
        public IList<CalendarParticipantModel> Participants { get; set; } = new List<CalendarParticipantModel>();
        public IList<CalendarTagModel> Tags { get; set; } = new List<CalendarTagModel>();
        public IList<CalendarAttributeModel> Attributes { get; set; } = new List<CalendarAttributeModel>();
    }
}