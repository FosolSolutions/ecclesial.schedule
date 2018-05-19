using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Ecclesial.Calendar.Helpers;
using Ecclesial.Calendar.Helpers.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecclesial.Calendar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]")]
    [Authorize]
    public class ParticipantController : Controller
    {
        #region Variables
        private readonly DataSource _context;
        private readonly MailClient _mailClient;
        #endregion

        #region Constructors
        public ParticipantController(DataSource context, MailClient mailClient)
        {
            _context = context;
            _mailClient = mailClient;
        }
        #endregion

        #region Endpoints
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult Participant(int id)
        {
            var participant = _context.Participants.Find(id);
            participant.Attributes = _context.ParticipantAttributes.Where(new { Participant_Id = id }).ToList();

            if (participant == null)
                return NoContent();

            return View(participant);
        }

        [HttpGet("add")]
        public IActionResult AddParticipant()
        {
            return View("Participant", new Participant());
        }

        [HttpGet("email/all/{scheduleId}")]
        public IActionResult EmailAllParticipants(int scheduleId)
        {
            var participants = _context.Participants.All();
            var errors = new List<Exception>();

            foreach ( var participant in participants)
            {
                if (!String.IsNullOrWhiteSpace(participant.Email))
                {
                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(1000);

                        try
                        {
                            _mailClient.Send(participant);
                        }
                        catch (Exception ex)
                        {
                            errors.Add(new Exception($"Mail failed for {participant.LastName}, {participant.FirstName}", ex));
                        }
                    });

                    t.Wait();
                }
            }

            if (errors.Count() == 0)
            {
                return new JsonResult(true);
            }

            return new JsonResult(errors.Select(e => new { e.Message, InnerException = e.InnerException.Message }));

        }
        #endregion
    }
}