using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.Filters
{
    public class ViewBagFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            ((Controller)context.Controller).ViewBag.Title = "Victoria Christadelphian Ecclesial Schedule";
        }
    }
}
