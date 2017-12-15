using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.Models
{
    public class CalendarModel
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte[] RowVersion { get; set; }
        public IList<CalendarMonthModel> Months { get; set; } = new List<CalendarMonthModel>();
        #endregion

        #region Constructors
        public CalendarModel()
        {

        }

        public CalendarModel(string title, DAL.Calendar calendar)
        {
            this.Id = calendar.Id;
            this.Name = calendar.Name;
            this.Title = title;
            this.StartDate = calendar.StartDate;
            this.EndDate = calendar.EndDate;
            this.RowVersion = calendar.RowVersion;
            this.Months = (from e in calendar.Events
                        group e by e.StartDate.Month into months
                        select new CalendarMonthModel()
                        {
                            Month = months.Key,
                            Name = $"{DateTimeFormatInfo.CurrentInfo.GetMonthName(months.Key)}",
                            Weeks = (from w in months
                                    group w by new { w.StartDate.Day, Name = DateTimeFormatInfo.CurrentInfo.GetDayName(w.StartDate.DayOfWeek) } into days
                                    select new CalendarWeekModel()
                                    {
                                        Day = days.Key.Day,
                                        Name = days.Key.Name,
                                        Events = (from e in days
                                                    group e by new { e.Id, e.Name, e.StartDate, e.EndDate, e.Description, e.RowVersion, e.Tasks } into events
                                                    select new CalendarEventModel()
                                                    {
                                                        Id = events.Key.Id,
                                                        StartDate = events.Key.StartDate,
                                                        EndDate = events.Key.EndDate,
                                                        Name = events.Key.Name,
                                                        Description = events.Key.Description,
                                                        RowVersion = events.Key.RowVersion,
                                                        Tasks = (from t in events.Key.Tasks
                                                                select new CalendarTaskModel()
                                                                {
                                                                    Id = t.Id,
                                                                    Name = t.Name,
                                                                    Description = t.Description,
                                                                    MinParticipants = t.MinParticipants,
                                                                    MaxParticipants = t.MaxParticipants,
                                                                    Participants = (from p in t.Participants
                                                                                    select new CalendarParticipantModel()
                                                                                    {
                                                                                        Id = p.Id,
                                                                                        FirstName = p.FirstName,
                                                                                        LastName = p.LastName,
                                                                                        Email = p.Email
                                                                                    }).ToList(),
                                                                    Tags = (from tag in t.Tags
                                                                            select new CalendarTagModel()
                                                                            {
                                                                                Id = tag.Id,
                                                                                Value = tag.Value,
                                                                                RowVersion = tag.RowVersion
                                                                            }).ToList(),
                                                                    Attributes = (from a in t.Attributes
                                                                                select new CalendarAttributeModel()
                                                                                {
                                                                                    Key = a.Key,
                                                                                    Value = a.Value,
                                                                                    Type = a.Type
                                                                                }).ToList(),
                                                                    RowVersion = t.RowVersion
                                                                }).ToList()
                                                    }).ToList()
                                    }).ToList()

                        }).ToList();
        }
        #endregion
    }
}
