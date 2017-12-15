using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ecclesial.Calendar.DAL
{
    public class CalendarTimeZone : BaseEntity
    {
        #region Properties
        [PrimaryKey, MaxLength(100)]
        public string Id { get; set; }

        [Required, MaxLength(100)]
        public string DisplayName { get; set; }

        [Required, MaxLength(100)]
        public string StandardName { get; set; }

        public TimeZoneInfo TimeZoneInfo
        {
            get { return TimeZoneInfo.GetSystemTimeZones().First(tz => String.Compare(tz.Id, this.Id, true) == 0); }
        }
        #endregion

        #region Constructors
        public CalendarTimeZone()
        {

        }

        public CalendarTimeZone(string id, string displayName = null, string standardName = null)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Cannot be null, empty or whitespace.", nameof(id));

            this.Id = id;
            this.DisplayName = displayName ?? id;
            this.StandardName = standardName ?? id;
        }

        public CalendarTimeZone(int id, TimeZoneInfo timezone)
            : this(timezone.Id, timezone.DisplayName, timezone.StandardName)
        {

        }
        #endregion
    }
}
