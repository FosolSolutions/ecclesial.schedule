using Fosol.Data.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.Filters
{
    public class JsonExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var status = HttpStatusCode.InternalServerError;
            var message = "An internal server error has occured.";

            var ex = context.Exception;
            var etype = ex.GetType();
            if (etype == typeof(NotFoundException))
            {
                status = HttpStatusCode.NoContent;
                message = "The content you requested does not exist.";
            }
            else if (etype == typeof(ConcurrencyException))
            {
                status = HttpStatusCode.BadRequest;
                message = "You attempted to update content that has been updated by someone else.  Please refresh your data and try again.";
            }
            else if (etype == typeof(DataException))
            {
                status = HttpStatusCode.InternalServerError;
                message = "An error has occured while attempting to save your change.  We apologize for the invonvience.";
            }
            else if (etype == typeof(UnauthorizedAccessException))
            {
                status = HttpStatusCode.Unauthorized;
                message = "You are unauthorized to peform this action.";
            }
            else if (etype == typeof(AuthenticationException)
                || etype == typeof(InvalidCredentialException))
            {
                status = HttpStatusCode.Forbidden;
                message = "You are not authenticated.";
            }
            else if (etype == typeof(SqlException))
            {
                switch (((SqlException)ex).Number)
                {
                    case (2627):
                        status = HttpStatusCode.BadRequest;
                        message = "You cannot insert duplicated records into the data source.";
                        break;
                    default:
                        status = HttpStatusCode.InternalServerError;
                        message = "An error has occured while attempting to save your change.  We apologize for the invonvience.";
                        break;
                }
            }

            var response = context.HttpContext.Response;
            response.StatusCode = (int)status;
            response.ContentType = "application/json";
            context.Result = new JsonResult(message);
            base.OnException(context);
        }
    }
}
