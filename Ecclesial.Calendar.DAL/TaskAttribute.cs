using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class TaskAttribute : BaseEntity
    {
        #region Properties

        [PrimaryKey(Order = 0), ForeignKey]
        public EventTask Task { get; set; }

        [PrimaryKey(Order = 1), MaxLength(50)]
        public string Key { get; set; }

        [PrimaryKey(Order = 2), MaxLength(250)]
        public string Value { get; set; }

        [Required]
        public TaskAttributeType Type { get; set; } = TaskAttributeType.Required;
        #endregion

        #region Constructors
        public TaskAttribute()
        {

        }

        public TaskAttribute(EventTask task, string key, string value, TaskAttributeType type)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(key));

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(value));

            this.Task = task;
            this.Key = key;
            this.Value = value;
            this.Type = type;
        }
        #endregion
    }
}
