using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class CalendarEvent : BaseEntity
    {
        #region Properties
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Required, DefaultValue("NEWID()")]
        public Guid Key { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(50)]
        public string TimezoneId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public CalendarEventStatus Status { get; set; } = CalendarEventStatus.Private;

        [Required, ForeignKey]
        public Calendar Calendar { get; set; }

        public List<EventTask> Tasks { get; set; } = new List<EventTask>();
        #endregion

        #region Constructors
        public CalendarEvent()
        {

        }

        public CalendarEvent(Calendar calendar, string name, DateTime start, DateTime end)
        {
            if (calendar == null)
                throw new ArgumentNullException(nameof(calendar));

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Cannot be null, empty or whitespace.", nameof(name));

            if (start > end)
                throw new ArgumentOutOfRangeException(nameof(end), "Start date must be before the end date.");

            this.Calendar = calendar;
            this.Key = Guid.NewGuid();
            this.Name = name;
            this.StartDate = start;
            this.EndDate = end;
            this.TimezoneId = TimeZoneInfo.Local.Id;
        }

        public CalendarEvent(Calendar calendar, string name, DateTime start, TimeSpan length) : this(calendar, name, start, start.Add(length))
        {
        }
        #endregion

        #region Methods
        public EventTask CreateTask(string name, int numberOfParticipants = 1)
        {
            return new EventTask(this, name, numberOfParticipants);
        }

        public EventTask AddTask(string name, int numberOfParticipants = 1)
        {
            var task = CreateTask(name, numberOfParticipants);
            this.Tasks.Add(task);
            return task;
        }
        #endregion
    }
}
