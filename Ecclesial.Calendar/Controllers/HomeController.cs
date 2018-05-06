using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecclesial.Calendar.Controllers
{
    public class HomeController : Controller
    {
        #region Variables
        private readonly DataSource _context;
        #endregion

        #region Constructors
        public HomeController(DataSource context)
        {
            _context = context;
        }
        #endregion

        #region Endpoints
        [HttpGet]
        public IActionResult Index()
        {
            var calendars = _context.Calendars.All();
            return View(calendars);
        }

        [HttpGet("home/thanks")]
        public IActionResult Thanks()
        {
            return Ok("Thank you for taking the time to volunteer and support the Victoria Ecclesia.");
        }
        #endregion
    }
}