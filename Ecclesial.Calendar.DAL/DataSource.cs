using Dapper;
using Fosol.Core.Extensions.DateTimes;
using Fosol.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ecclesial.Calendar.DAL
{
    [DataSource("EccSchedule")]
    public class DataSource : DataContext
    {
        #region Variables
        #endregion

        #region Properties
        public Repository<Tag> Tags { get; set; }
        public Repository<Attribute> Attributes { get; set; }
        public Repository<CalendarTimeZone> TimeZones { get; set; }
        public Repository<Calendar> Calendars { get; set; }
        public Repository<CalendarEvent> Events { get; set; }
        public Repository<EventTask> Tasks { get; set; }
        public Repository<TaskAttribute> TaskAttributes { get; set; }
        public Repository<TaskTag> TaskTags { get; set; }
        public Repository<Participant> Participants { get; set; }
        public Repository<ParticipantAttribute> ParticipantAttributes { get; set; }
        public Repository<ParticipantEventTask> ParticipantEventTasks { get; set; }
        #endregion

        #region Constructors
        public DataSource() : base("Ecclesial.Calendar")
        {
        }

        public DataSource(string connectionString) : base(connectionString)
        {
        }

        public DataSource(DataSourceOptions options) : base(options)
        {
        }

        public DataSource(IOptions<DataSourceOptions> options) : base(options)
        {
        }
        #endregion

        #region Methods
        protected override void Seed()
        {
            // Insert TimeZones.
            var system_time_zones = TimeZoneInfo.GetSystemTimeZones().ToArray();
            var timezones = new List<CalendarTimeZone>(system_time_zones.Count());

            for (var i = 0; i < system_time_zones.Count(); i++)
            {
                timezones.Add(new CalendarTimeZone(i+1, system_time_zones[i]));
            }

            this.TimeZones.Add(timezones);

            // Insert Attributes.
            this.Attributes.Add(new[] {
                new Attribute("Role", "Brother"),
                new Attribute("Role", "Sister"),
                new Attribute("Role", "AB"),
                new Attribute("Role", "Presider"),
                new Attribute("Role", "Speaker"),
                new Attribute("Role", "Pianist"),
                new Attribute("Role", "Server"),
                new Attribute("Role", "Reader"),
                new Attribute("Role", "Doorkeeper") });

            // Insert TaskTags.
            this.Tags.Add(new[] {
                new Tag("Topic") });

            // Insert Calendar.
            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 6, 30).EndOfDay();
            var calendar = new Calendar("Victoria Christadelphian Ecclesia", start, end)
            {
                Description = $"Plan of Appointments - January to June 2018 - God Willing"
            };
            this.Calendars.Add(calendar);

            // Insert Events.

            //-------------------------------------------------------
            // Sunday Meetings
            //-------------------------------------------------------
            var sunday = start.AddDays(start.DayOfWeek == DayOfWeek.Sunday ? 0 : 7 - (int)start.DayOfWeek);
            while (sunday <= end)
            {
                // Memorial
                var memorial = calendar.AddEvent("Memorial", sunday.SetTime(11), sunday.SetTime(13));
                this.Events.Add(memorial);

                var preside = memorial.AddTask("Preside");
                preside.AddRequiredAttribute("Role", "Brother");
                preside.AddRequiredAttribute("Role", "Presider");

                var exhort = memorial.AddTask("Exhort");
                exhort.AddRequiredAttribute("Role", "Brother");
                exhort.AddRequiredAttribute("Role", "Speaker");

                var pianist = memorial.AddTask("Pianist");
                pianist.AddRequiredAttribute("Role", "Pianist");

                var read1 = memorial.AddTask("Reading #1");
                read1.AddRequiredAttribute("Role", "Brother");

                var read2 = memorial.AddTask("Reading #2");
                read2.AddRequiredAttribute("Role", "Brother");

                var bread = memorial.AddTask("Bread");
                bread.AddRequiredAttribute("Role", "Brother");

                var wine = memorial.AddTask("Wine");
                wine.AddRequiredAttribute("Role", "Brother");

                var close = memorial.AddTask("Close");
                close.AddRequiredAttribute("Role", "Brother");

                var door = memorial.AddTask("Door");
                door.AddRequiredAttribute("Role", "Brother");
                door.AddRequiredAttribute("Role", "Doorkeeper");

                this.Tasks.Add(memorial.Tasks);

                foreach (var ce in memorial.Tasks)
                {
                    this.TaskAttributes.Add(ce.Attributes);
                    this.TaskTags.Add(ce.Tags);
                }

                // Lecture
                var lecture = calendar.AddEvent("Lecture", sunday.SetTime(18), sunday.SetTime(19));
                this.Events.Add(lecture);

                var speak = lecture.AddTask("Lecture");
                speak.AddRequiredAttribute("Role", "Brother");
                speak.AddRequiredAttribute("Role", "Speaker");
                speak.AddTag("Topic", null);

                this.Tasks.Add(lecture.Tasks);

                foreach (var ce in lecture.Tasks)
                {
                    this.TaskAttributes.Add(ce.Attributes);
                    this.TaskTags.Add(ce.Tags);
                }

                sunday = sunday.AddDays(7);
            }

            //-------------------------------------------------------
            // Bible Classes
            //-------------------------------------------------------
            var thursday = start.AddDays(start.DayOfWeek == DayOfWeek.Thursday ? 0 : 4 - (int)start.DayOfWeek);

            while (thursday <= end)
            {
                var bible = calendar.AddEvent("Bible Talk", thursday.SetTime(18, 30), thursday.SetTime(19, 30));
                this.Events.Add(bible);

                var pianist = bible.AddTask("Pianist");
                pianist.AddRequiredAttribute("Role", "Pianist");

                var speak = bible.AddTask("Speaker");
                speak.AddRequiredAttribute("Role", "Brother");
                speak.AddTag("Topic", null);

                this.Tasks.Add(bible.Tasks);

                foreach (var ce in bible.Tasks)
                {
                    this.TaskAttributes.Add(ce.Attributes);
                    this.TaskTags.Add(ce.Tags);
                }

                thursday = thursday.AddDays(7);
            }


            //-------------------------------------------------------
            // Cleaning Schedule
            //-------------------------------------------------------
            var saturday = start.AddDays(start.DayOfWeek == DayOfWeek.Saturday ? 0 : 6 - (int)start.DayOfWeek);

            while (saturday <= end)
            {
                var cleaning = calendar.AddEvent("Janitorial Duties", saturday.SetTime(9), saturday.SetTime(11));
                this.Events.Add(cleaning);

                var clean = cleaning.AddTask("Clean", 6);
                this.Tasks.Add(cleaning.Tasks);

                foreach (var ce in cleaning.Tasks)
                {
                    this.TaskAttributes.Add(ce.Attributes);
                    this.TaskTags.Add(ce.Tags);
                }

                saturday = saturday.AddDays(7);
            }

            //-------------------------------------------------------
            // Participants
            //-------------------------------------------------------
            this.Participants.Add(new Participant[] {
                new Participant("Lynette Alderson", "lynettealderson@shaw.ca", new { Key = "Role", Value = "Sister" }) { Gender = Gender.Female },
                new Participant("Rosa Bailey", "rosabailey@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Stephen Bennett", "stephen.bennett01@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Joan Bennett", "joanimbennett@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Matthew Bennett", "mattsbennett@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Roberta Cadieu", "rjcadieu@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Larry Catchpole", "NA", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Debbie Catchpole", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Dan Cawston", "dancaw@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Tiana Cawston", "dancaw@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Andrea Ceron", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Daniel Cover", "danielclover@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Carita Clover", "danielclover@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Hannah Clover", "hannahclover21@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Jack Clover", "jayclo@hotmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Valerie Clover", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Jeni Coupar", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Beth Dangerfield", "gdanger@shaw.ca", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Caitlyn Daniel", "NA", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Clive Daniel", "NA", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Jennifer Daniel", "jenniferdaniel@telus.net", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Eileen Daniel", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Justin Enns", "justin-enns-@outlook.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Mary Enns", "maryclover@live.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Naleen Fernando", "naleenandjulie@gmail.com", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Gregg Ferrie", "gferrie@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Vikki Ferrie", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Donna Foss", "dfoss@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Jeremy Foster", "jeremymfoster@hotmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Elizabeth Foster", "ejoyfoster@gmail.com", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Linda Gilmore", "linda.gilmour@telus.net", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Diana Gorman", "dianagorman@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Alex Harper", "aharper10@hotmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Art Hibbs", "art.hibbs@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Linda Hibbs", "2hibbs@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Jeff Hibbs", "jeffvictoria09@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Pianist" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Victoria Hibbs", "jeffvictoria09@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Diane Hills", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Kevin Hunter", "hunterfamily9@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Rebekah Hunter", "hunterfamily9@gmail.com", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Cheryalee Hutchison", "cheryalee@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("William Hutchison", "whutchis@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Elaine Jennings", "institches56@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Rod Johnston", "rod.johnston@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Elizabeth Johnston", "elizabeth.johnston@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Carolyn Jones", "jonesbc@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Lorraine Kemp", "lorrainekemp@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Joanne Kirk", "tekjak@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Denise Knorr", "denise00014@hotmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Peter Lawrence", "joshua@joshualawrence.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Hannah Lawrence", "hlawrence005@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Mark Little", "zhq.6987@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Zoe Little", "mwl@netzero.net", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Myra Lucke", "myralucke1@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Horace Macpherson", "smmacpherson4@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Sylvia Macpherson", "smmacpherson4@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Bertha McArthur", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Joshua McStravick", "joshmcstravick@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Leah McStravick", "leah.mcstravick@gmail.com", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Mike McStravick", "mtmcstravick@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Sandra McStravick", "ssmcstravick@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Janis Morrison", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Jamie Myren", "jamie_my@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Joe Myren", "joseph798185@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Trish Myren", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Matthew Neville", "matthew.n.sabrina@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Sabrina Neville", "matthew.n.sabrina@gmail.com", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Alan Ormerod", "aormerod@shaw.ca", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Rodney Owens", "rodowens@shaw.ca", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Margaret Payne", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Peggy Pearce", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Laura Pengelly", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Marianne Pilon", "mppilon@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Micah Quindazzi", "NA", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Philip Quindazzi", "NA", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Andrew Ralph", "drew.sherry@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Sherry Ralph", "drew.sherry@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Paul Ratzka", "paulratzka@yahoo.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Bonny Ratzka", "sueme38@hotmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Joan Rebman", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Grace Salisbury", "normgrace@live.ca", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Anne Sandoval", "anneksandoval@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Grace Shrimpton", "gmcshrimp@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Wendy Shrimpton", "whitemore@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Clyde Snobelen", "csnobelen@csll.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Evelyn Snobelen", "csnobelen@csll.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Chase Snobelen", "chase.snobelen@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Meagan Snobelen", "msmcstravick@gmail.com", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Mark Snobelen", "snobelens@shaw.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Sherry Snobelen", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Shawn Snobelen", "snobelen@gmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "AB" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Marnie Snobelen", "marnieleigh@gmail.com", new { Key = "Role", Value = "Sister" }, new { Key = "Role", Value = "Pianist" })  { Gender = Gender.Female },
                new Participant("Taleigh Snobelen", "taleigh.rae@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Al Starcher", "NA", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Bob Stodel", "rwstodel@telus.net", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Speaker" }, new { Key = "Role", Value = "Presider" })  { Gender = Gender.Male },
                new Participant("Dianna Stodel", "dianastodel@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Vic Stodel", "marie-vics@shaw.ca", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Marie Stodel", "marie-vics@shaw.ca", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Rob Wallace", "NA", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Linda Wallace", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Pat Williamson", "pwilliamson369@gmail.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Pat Willimont", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Arthur Wood", "arthurwood@outlook.com", new { Key = "Role", Value = "Brother" })  { Gender = Gender.Male },
                new Participant("Sharon Wood", "arthurwood@outlook.com", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Joan Woodcock", "NA", new { Key = "Role", Value = "Sister" })  { Gender = Gender.Female },
                new Participant("Stephen Higgs", "stevehiggs@live.ca", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" })  { Gender = Gender.Male },
                new Participant("Linda Higgs", "lin.susan.lh@gmail.com", new { Key = "Role", Value = "Sister" })   { Gender = Gender.Female },
                new Participant("Judy Hewer", "judyhewer@hotmail.ca", new { Key = "Role", Value = "Sister" })   { Gender = Gender.Female },
                new Participant("Sandi Coleswebb", "s.coleswebb@gmail.com", new { Key = "Role", Value = "Sister" })   { Gender = Gender.Female },
                new Participant("Mark Macfarlane", "macfarlane.9@hotmail.com", new { Key = "Role", Value = "Brother" }, new { Key = "Role", Value = "Presider" }, new { Key = "Role", Value = "Doorkeeper" }, new { Key = "Role", Value = "Speaker" })  { Gender = Gender.Male }
            });

            foreach (var participant in this.Participants)
            {
                this.ParticipantAttributes.Add(participant.Attributes);
            }
        }
        #endregion
    }
}
