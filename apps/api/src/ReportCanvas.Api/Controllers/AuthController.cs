using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReportCanvas.Application.Features.Auth.DTOs;
using ReportCanvas.Domain.Entities;
using ReportCanvas.Infrastructure.Persistence;

namespace ReportCanvas.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly ReportCanvasDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config,
        ReportCanvasDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        // Create organization and workspace for the new user
        var org = new Organization
        {
            Name = request.OrganizationName,
            Slug = Slugify(request.OrganizationName)
        };
        _db.Organizations.Add(org);

        var workspace = new Workspace
        {
            Name = "My Workspace",
            Slug = "my-workspace",
            OrganizationId = org.Id
        };
        _db.Workspaces.Add(workspace);

        var membership = new Membership
        {
            UserId = Guid.Parse(user.Id),
            UserEmail = user.Email!,
            OrganizationId = org.Id,
            Role = Domain.Enums.MembershipRole.Owner
        };
        _db.Memberships.Add(membership);

        await _db.SaveChangesAsync();

        var token = GenerateJwt(user);
        return Ok(BuildAuthResponse(token, user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials." });

        var token = GenerateJwt(user);
        return Ok(BuildAuthResponse(token, user));
    }

    private string GenerateJwt(ApplicationUser user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresIn = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "1440");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("fullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static AuthResponse BuildAuthResponse(string token, ApplicationUser user)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var expiresIn = (int)(jwt.ValidTo - DateTime.UtcNow).TotalSeconds;

        return new AuthResponse(
            AccessToken: token,
            TokenType: "Bearer",
            ExpiresIn: expiresIn,
            User: new UserDto(user.Id, user.Email!, user.FullName));
    }

    private static string Slugify(string input) =>
        new string(input.ToLowerInvariant()
            .Replace(' ', '-')
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .ToArray());
}
