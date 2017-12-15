using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class CalendarDay : BaseEntity
    {
        #region Properties
        [Required]
        public DateTime Date { get; set; }

        public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
        #endregion

        #region Constructors
        public CalendarDay()
        {

        }

        public CalendarDay(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException(nameof(date));

            this.Date = date;
        }
        #endregion
    }
}
