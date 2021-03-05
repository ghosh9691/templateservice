using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TemplateService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemplateService.Attributes
{
    public class TransactionHeaderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as BaseController;
            if (controller != null)
            {
                var headers = context.HttpContext.Request.Headers;
                headers.TryGetValue("request-id", out StringValues values);
                if (values.Count <= 0)
                {
                    controller.RequestId = Guid.NewGuid().ToString("N");
                    headers.Add("request-id", controller.RequestId);
                }
                else
                {
                    controller.RequestId = values[0];
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
