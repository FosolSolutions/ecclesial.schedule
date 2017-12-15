using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class EventTask : BaseEntity
    {
        #region Properties
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required, ForeignKey]
        public CalendarEvent Event { get; set; }

        public IList<TaskAttribute> Attributes { get; set; } = new List<TaskAttribute>();

        public IList<TaskTag> Tags { get; set; } = new List<TaskTag>();

        public int MinParticipants { get; set; } = 1;

        public int MaxParticipants { get; set; } = 1;

        public IList<Participant> Participants { get; set; } = new List<Participant>();
        #endregion

        #region Constructors
        public EventTask()
        {

        }

        public EventTask(CalendarEvent calendarEvent, string name, int numberOfParticipants = 1)
        {
            if (calendarEvent == null)
                throw new ArgumentNullException(nameof(calendarEvent));

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must not be null, empty or whitespace.", nameof(name));

            if (numberOfParticipants < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfParticipants), "Must be greater or equal to 1.");

            this.Name = name;
            this.Event = calendarEvent;
            this.MaxParticipants = numberOfParticipants;
        }
        #endregion

        #region Methods
        public TaskAttribute AddRequiredAttribute(string key, string value)
        {
            var attr = new TaskAttribute(this, key, value, TaskAttributeType.Required);
            this.Attributes.Add(attr);

            return attr;
        }

        public TaskAttribute AddOptionalAttribute(string key, string value)
        {
            var attr = new TaskAttribute(this, key, value, TaskAttributeType.Optional);
            this.Attributes.Add(attr);

            return attr;
        }

        public TaskAttribute AddDisallowedAttribute(string key, string value)
        {
            var attr = new TaskAttribute(this, key, value, TaskAttributeType.Disallowed);
            this.Attributes.Add(attr);

            return attr;
        }

        public TaskTag AddTag(string id, object value)
        {
            var tag = new TaskTag(this, id, value);
            this.Tags.Add(tag);
            return tag;
        }
        #endregion
    }
}
