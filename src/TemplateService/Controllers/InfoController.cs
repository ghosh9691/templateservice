using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TemplateService.Models;
using Refit;
using TemplateService.Interfaces;

namespace TemplateService.Controllers
{
    [Route("")]
    public class InfoController : BaseController
    {
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetInfo()
        {
            return Ok(new
            {
                Message = "Template Web Service 1.0",
                CurrentMinimumLogLevel = Program.LogLevelSwitch.MinimumLevel,
                AvailableAt = await GetMyExternalIP()
            });
        }

        private async Task<string> GetMyExternalIP()
        {
            var client = RestService.For<IExternalIp>("http://ipv4.icanhazip.com/");
            var myIp = await client.GetMyExternalIp();
            return myIp.Trim();
        }

        [SwaggerOperation(
            Summary = "Dynamically changes the log level - helpful for debugging",
            Description = "Dynamically change the log leve",
            OperationId = "ChangeLogLevel"
        )]
        [HttpPost("loglevel")]
        public async Task<IActionResult> ChangeLogLevel([FromBody] LogLevelRequest logLevelRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Unknown log level");
            }

            await Task.Run(() => { Program.LogLevelSwitch.MinimumLevel = logLevelRequest.LogLevel; });
            return Ok();
        }
    }
}
