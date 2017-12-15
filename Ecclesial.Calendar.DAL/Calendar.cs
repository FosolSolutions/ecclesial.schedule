using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class Calendar : BaseEntity
    {
        #region Properties
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Required, DefaultValue("NEWID()")]
        public Guid Key { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public CalendarStatus Status { get; set; } = CalendarStatus.Private;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(100)]
        public string TimeZoneId { get; set; }

        [ForeignKey(Columns = new[] { nameof(TimeZoneId) })]
        public CalendarTimeZone TimeZone { get; set; }

        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();

        public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
        #endregion

        #region Constructors
        public Calendar()
        {
        }

        public Calendar(string name, DateTime startDate, DateTime endDate, TimeZoneInfo timezone)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Cannot be null, empty or whitespace.", nameof(name));

            this.Name = name;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.TimeZoneId = timezone.Id;
            this.Key = Guid.NewGuid();
        }
        public Calendar(string name, DateTime startDate, DateTime endDate) : this(name, startDate, endDate, TimeZoneInfo.Local)
        {

        }

        public Calendar(string name, int year, TimeZoneInfo timezone) : this(name, new DateTime(year, 1, 1), new DateTime(year, 12, 31, 23, 59, 59, 999), timezone)
        {

        }

        public Calendar(string name, int year) : this(name, year, TimeZoneInfo.Local)
        {

        }
        #endregion

        #region Methods
        public CalendarEvent CreateEvent(string name, DateTime start, DateTime end)
        {
            return new CalendarEvent(this, name, start, end);
        }

        public CalendarEvent CreateEvent(string name, DateTime start, TimeSpan length)
        {
            return new CalendarEvent(this, name, start, length);
        }

        public CalendarEvent AddEvent(string name, DateTime start, DateTime end)
        {
            var e = CreateEvent(name, start, end);
            this.Events.Add(e);
            return e;
        }

        public CalendarEvent AddEvent(string name, DateTime start, TimeSpan length)
        {
            var e = CreateEvent(name, start, length);
            this.Events.Add(e);
            return e;
        }
        #endregion
    }
}
