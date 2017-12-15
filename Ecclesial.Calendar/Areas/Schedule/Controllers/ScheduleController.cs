using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Ecclesial.Calendar.Helpers;
using Fosol.Core.Extensions.IEnumerables;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecclesial.Calendar.Areas.Schedule.Controllers
{
    [Area("Schedule")]
    [Route("[area]")]
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
        [HttpGet("{id}")]
        public IActionResult Index(int id)
        {
            var calendar = _context.Calendars.Find(id);

            if (calendar == null)
                return NoContent();

            return View(calendar);
        }
        #endregion
    }
}