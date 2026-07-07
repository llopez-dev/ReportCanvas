using Microsoft.AspNetCore.Mvc;

namespace ReportCanvas.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        version = "0.1.0"
    });
}
