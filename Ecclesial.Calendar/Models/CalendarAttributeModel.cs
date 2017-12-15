namespace Ecclesial.Calendar.Models
{
    public class CalendarAttributeModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DAL.TaskAttributeType Type { get; set; }
    }
}