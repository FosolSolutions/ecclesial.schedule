using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Ecclesial.Calendar.Helpers;
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
        #endregion

        #region Constructors
        public ParticipantController(DataSource context)
        {
            _context = context;
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
        #endregion
    }
}