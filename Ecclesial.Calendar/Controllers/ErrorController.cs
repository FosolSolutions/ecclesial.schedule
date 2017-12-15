using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecclesial.Calendar.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecclesial.Calendar.Controllers
{
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        #region Variables
        private readonly DataSource _context;
        #endregion

        #region Constructors
        public ErrorController(DataSource context)
        {
            _context = context;
        }
        #endregion

        #region Endpoints
        [HttpGet("{statusCode?}")]
        public IActionResult Index(int statusCode)
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            switch (statusCode)
            {
                case ((int)HttpStatusCode.NotFound):
                    return View("404-NotFound");
                case ((int)HttpStatusCode.Unauthorized):
                    return View("401-Unauthorized");
                case ((int)HttpStatusCode.Forbidden):
                    return View("403-Forbidden");
                case ((int)HttpStatusCode.InternalServerError):
                default:
                    return View();
            }
        }
        #endregion
    }
}