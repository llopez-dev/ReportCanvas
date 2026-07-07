using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportCanvas.Application.Common.Interfaces;
using ReportCanvas.Application.Features.Auth.DTOs;

namespace ReportCanvas.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public MeController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet]
    public IActionResult Get() => Ok(new UserDto(
        _currentUser.UserId,
        _currentUser.Email,
        string.Empty // FullName added in next iteration when we expand UserDto
    ));
}
