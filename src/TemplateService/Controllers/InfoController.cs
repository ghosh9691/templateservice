using Microsoft.AspNetCore.Mvc;

namespace TemplateService.Controllers;

[ApiController]
[Route("")]
public class InfoController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetInfo()
    {
        var result = await Task.Run(() => new
        {
            Name = "Pyxis International Aircloud Template Service",
            Version = "1.0.0",
            Environment = GetEnvironmentVariable()
        });
        return Ok(result);
    }

    private string? GetEnvironmentVariable()
    {
        return Environment.GetEnvironmentVariable("Settings__Value");
    }
}