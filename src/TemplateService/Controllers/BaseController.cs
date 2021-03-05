using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Attributes;

namespace TemplateService.Controllers
{
    [ApiController]
    [OperationId]
    [TransactionHeader]
    [UserContext]
    public class BaseController : ControllerBase
    {
        public string RequestId { get; set; }
        public  string CurrentToken { get; set; }
        public string CurrentUser { get; set; }
    }
}
