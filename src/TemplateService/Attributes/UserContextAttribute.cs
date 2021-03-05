using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using PrabalGhosh.Utilities;
using Serilog;
using TemplateService.Controllers;

namespace TemplateService.Attributes
{
    public class UserContextAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var httpContext = context.HttpContext;
                if (httpContext.IsNotNull())
                {
                    var controller = context.Controller as BaseController;
                    if (controller.IsNotNull())
                    {
                        var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier));
                        controller.CurrentUser = claim.Value;
                        Log.Debug($"Extracted NameIdentity claim {claim.Value}");
                        controller.CurrentToken = httpContext.Request.Headers[HeaderNames.Authorization];
                        Log.Debug($"Extracted Token: {controller.CurrentToken}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}