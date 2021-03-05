using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using PrabalGhosh.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace TemplateService.Attributes
{
    public class OperationIdAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (actionDescriptor.IsNotNull())
                {
                    var value = actionDescriptor.AttributeRouteInfo.Name;
                    if (!context.HttpContext.Response.Headers.ContainsKey("operation-id") && value.IsNotNull())
                        context.HttpContext.Response.Headers.Add("operation-id", value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}