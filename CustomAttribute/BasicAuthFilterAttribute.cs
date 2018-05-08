using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.CustomAttribute
{
    internal class BasicAuthFilterAttribute : ActionFilterAttribute
    {
        private StringValues xyz;
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var authHeader = actionContext.HttpContext.Request.Headers.TryGetValue("Authorization", out xyz);
        }
    }
}
