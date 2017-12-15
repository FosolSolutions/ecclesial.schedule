using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Ecclesial.Calendar.Filters;
using Ecclesial.Calendar.Helpers;
using Fosol.Data.Exceptions;
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
        [HttpGet, JsonExceptionFilter]
        public IActionResult GetCurrentParticipant()
        {
            var participant = User.GetParticipant(_context);
            participant.Attributes = _context.ParticipantAttributes.Where(new { Participant_Id = participant.Id }).ToList();
            return Ok(participant);
        }

        [HttpGet("/[area]/participants"), JsonExceptionFilter]
        public IActionResult GetParticipants([FromQuery] int page = 0, [FromQuery]  int qty = 20)
        {
            if (page < 0 || qty < 0)
                return BadRequest("Invalid parameters");

            var participants = _context.Participants.Query($"SELECT * FROM dbo.Participant p ORDER BY p.[LastName], p.[FirstName] OFFSET {page * qty} ROWS FETCH NEXT {qty} ROWS ONLY").ToList();

            return Ok(participants);
        }

        [HttpGet("{id}"), JsonExceptionFilter]
        public IActionResult GetParticipant(int id)
        {
            var participant = _context.Participants.Find(id);

            if (participant == null)
                return NoContent();

            return Ok(participant);
        }

        [HttpPost, JsonExceptionFilter]
        public IActionResult AddParticipant([FromBody] Participant participant)
        {
            if (participant == null)
                return BadRequest();

            participant.Key = Guid.NewGuid();

            _context.Participants.Add(participant);

            return Created($"/data/participant/{participant.Id}", participant);
        }

        [HttpPut, JsonExceptionFilter]
        public IActionResult UpdateParticipant([FromBody] Participant participant)
        {
            if (participant == null)
                return BadRequest();

            _context.Participants.Update(participant);

            return Ok(participant);
        }

        [HttpDelete, JsonExceptionFilter]
        public IActionResult DeleteParticipant([FromBody] Participant participant)
        {
            if (participant == null)
                return BadRequest();

            _context.Participants.Delete(participant);

            return Ok();
        }

        [HttpGet("attributes/{participantId}"), JsonExceptionFilter]
        public IActionResult GetAttribute(int participantId)
        {
            var participant = _context.Participants.Find(participantId);

            if (participant == null)
                return BadRequest();

            var attributes = _context.ParticipantAttributes.Where(new { Participant_Id = participantId });

            return Ok(attributes);
        }

        [HttpGet("attribute/{participantId}/{key}/{value}"), JsonExceptionFilter]
        public IActionResult GetAttribute(int participantId, string key, string value)
        {
            if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(value))
                return BadRequest();

            var participant = _context.Participants.Find(participantId);

            if (participant == null)
                return BadRequest();

            var attribute = _context.ParticipantAttributes.Find(participantId, key, value);
            attribute.Participant = participant;

            return Ok(attribute);
        }

        [HttpPost("attribute"), JsonExceptionFilter]
        public IActionResult AddAttribute([FromBody] ParticipantAttribute attribute)
        {
            if (attribute == null || attribute.Participant == null)
                return BadRequest("Invalid attribute");

            var participant = _context.Participants.Find(attribute.Participant.Id);

            if (participant == null)
                return BadRequest();

            _context.ParticipantAttributes.Add(attribute);

            return Created($"/data/participant/attribute/{participant.Id}/{attribute.Key}/{attribute.Value}", attribute);
        }

        [HttpPut("attribute"), JsonExceptionFilter]
        public IActionResult UpdateAttribute([FromBody] ParticipantAttribute attribute)
        {
            if (attribute == null || attribute.Participant == null)
                return BadRequest();

            var participant = _context.Participants.Find(attribute.Participant.Id);

            if (participant == null)
                return BadRequest();

            _context.ParticipantAttributes.Update(attribute);

            return Ok(attribute);
        }

        [HttpDelete("attribute"), JsonExceptionFilter]
        public IActionResult DeleteAttribute([FromBody] ParticipantAttribute attribute)
        {
            if (attribute == null || attribute.Participant == null)
                return BadRequest();

            var participant = _context.Participants.Find(attribute.Participant.Id);

            if (participant == null)
                return BadRequest();

            _context.ParticipantAttributes.Delete(attribute);

            return Ok("{}");
        }
        #endregion
    }
}