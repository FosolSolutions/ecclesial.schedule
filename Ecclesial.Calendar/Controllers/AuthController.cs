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

namespace Ecclesial.Calendar.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        #region Variables
        private readonly DataSource _context;
        #endregion

        #region Constructors
        public AuthController(DataSource context)
        {
            _context = context;
        }
        #endregion

        #region Endpoints
        [HttpGet("signin")]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpGet("validate/user/{key}")]
        public async Task<IActionResult> ValidateUser(Guid key)
        {
            var identity = _context.CreateIdentity(key);
            if (identity == null)
                return Unauthorized();

            var user = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("signoff"), Authorize]
        public async Task<IActionResult> SignOff()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Thanks", "Home");
        }

        [HttpGet("jeremy")]
        public IActionResult Backdoor()
        {
            var participant = _context.Participants.Where(new { FirstName = "Jeremy", LastName = "Foster" }).FirstOrDefault();

            if (participant == null)
                return BadRequest();

            return RedirectToAction("ValidateUser", new { key = participant.Key });
        }

        [HttpGet("impersonate"), Authorize]
        public IActionResult Impersonate()
        {
            var participants = _context.Participants.All();
            return View(participants);
        }

        [HttpGet("impersonate/{key}"), Authorize]
        public async Task<IActionResult> Impersonate(Guid key)
        {
            var userId = int.Parse(User.GetNameIdentifier().Value);
            var participant = _context.Participants.Where(new { Key = key }).FirstOrDefault();
            if (participant == null)
                return BadRequest();

            var identity = _context.CreateIdentity(participant.Id);
            if (identity == null)
                return Unauthorized();

            var id = identity.GetNameIdentifier();
            identity.RemoveClaim(id);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, $"{userId}"));
            identity.AddClaim(new Claim("Impersonate", $"{participant.Id}", "int", "Ecclesial.Calendar"));
            var user = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}