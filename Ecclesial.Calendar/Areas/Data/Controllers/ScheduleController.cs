using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Ecclesial.Calendar.Filters;
using Ecclesial.Calendar.Helpers;
using Fosol.Core.Extensions.IEnumerables;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecclesial.Calendar.Areas.Data.Controllers
{
    [Produces("application/json")]
    [Area("Data")]
    [Route("[area]/[controller]")]
    [Authorize]
    public class ScheduleController : Controller
    {
        #region Variables
        private readonly DataSource _context;
        #endregion

        #region Constructors
        public ScheduleController(DataSource context)
        {
            _context = context;
        }
        #endregion

        #region Endpoints
        [HttpGet("{id}/{weekday?}"), AllowAnonymous, JsonExceptionFilter]
        public IActionResult Schedule(int id, DayOfWeek weekday = DayOfWeek.Sunday)
        {
            var calendar = _context.Calendars.Find(id);

            if (calendar == null)
                return NoContent();

            calendar.Events = _context.Events.Where("[Calendar_Id]=@Calendar_Id AND DATEPART(dw, [StartDate])=@WeekDay", new { Calendar_Id = id, WeekDay = (int)weekday + 1 }).ToList();

            foreach (var cevent in calendar.Events)
            {
                cevent.Tasks = _context.Tasks.Where(new { CalendarEvent_Id = cevent.Id }).ToList();

                foreach (var task in cevent.Tasks)
                {
                    task.Tags = _context.TaskTags.Where(new { EventTask_Id = task.Id }).ToList();
                    task.Attributes = _context.TaskAttributes.Where(new { EventTask_Id = task.Id }).ToList();
                    task.Participants = _context.Participants.Query($"SELECT p.* FROM [dbo].[Participant] p INNER JOIN [dbo].[ParticipantEventTask] pet ON p.Id = pet.Participant_Id WHERE pet.[EventTask_Id]=@Id", new { task.Id }).ToList();
                }
            }

            var result = new Models.CalendarModel("Sunday Schedule", calendar);

            return Ok(result);
        }

        [HttpPost("accept/task/{id}"), JsonExceptionFilter]
        public IActionResult AcceptTask(int id)
        {
            var task = _context.Tasks.Find(id);

            if (task == null)
                return BadRequest("Invalid parameter");

            if (!User.IsAllowed(_context, id))
                return Forbid();

            try
            {
                var participant = User.GetParticipant(_context);

                task.Participants = _context.Participants.Query(ParticipantEventTask.SelectParticipantsSql(), new { task.Id }).ToList();
                if (task.MaxParticipants <= task.Participants.Count())
                    return BadRequest("Someone else has already volunteered for this task while you were thinking.  Better luck next time!");

                _context.Execute(ParticipantEventTask.InsertSql(participant, task));

                task.Tags = _context.TaskTags.Where(new { EventTask_Id = task.Id }).ToList();
                task.Attributes = _context.TaskAttributes.Where(new { EventTask_Id = task.Id }).ToList();
                task.Participants = _context.Participants.Query(ParticipantEventTask.SelectParticipantsSql(), new { task.Id }).ToList();

                return Ok(new { Task = task, Participant = participant });
            }
            catch (SqlException e)
            {
                if (e.Number == 2627)
                {
                    return BadRequest("You have already volunteered for this task.");
                }
                return StatusCode(500, "An error has occured while attempting to save your change.  We apologize for the invonvience.");
            }
        }

        [HttpDelete("decline/task/{id}"), JsonExceptionFilter]
        public IActionResult DeclineTask(int id)
        {
            var task = _context.Tasks.Find(id);

            if (task == null)
                return BadRequest();

            if (!User.IsAllowed(_context, id))
                return Forbid();

            var participant = User.GetParticipant(_context);
            _context.Execute(ParticipantEventTask.DeleteSql(participant, task));

            task.Tags = _context.TaskTags.Where(new { EventTask_Id = task.Id }).ToList();

            task.Tags.ForEach((t) => {
                t.Task = task;
                t.Value = null;
                _context.TaskTags.Update(t);
            });

            task.Attributes = _context.TaskAttributes.Where(new { EventTask_Id = task.Id }).ToList();
            task.Participants = _context.Participants.Query(ParticipantEventTask.SelectParticipantsSql(), new { task.Id }).ToList();

            return Ok(new { Task = task, Participant = participant });
        }

        [HttpPut("tag"), JsonExceptionFilter]
        public IActionResult SaveTag(int taskId, string id, string value, string rowVersion)
        {
            if (taskId == 0 || String.IsNullOrWhiteSpace(id))
                return BadRequest();

            var task = _context.Tasks.Find(taskId);
            var otag = _context.TaskTags.Find(taskId, id);

            if (otag == null)
                return BadRequest();

            otag.Value = value;
            otag.RowVersion = Convert.FromBase64String(rowVersion);
            otag.Task = task;

            _context.TaskTags.Update(otag);

            return Ok(otag);
        }
        #endregion
    }
}