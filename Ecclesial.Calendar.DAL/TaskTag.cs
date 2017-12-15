using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class TaskTag : BaseEntity
    {
        #region Properties

        [PrimaryKey(Order = 0), ForeignKey]
        public EventTask Task { get; set; }

        [PrimaryKey(Order = 1), MaxLength(100)]
        public string Id { get; set; }

        [MaxLength(1000)]
        public string Value { get; set; }
        #endregion

        #region Constructors
        public TaskTag()
        {

        }

        public TaskTag(EventTask task, string id, object value)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(id));

            this.Task = task;
            this.Id = id;
            this.Value = $"{value}";
        }
        #endregion
    }
}
