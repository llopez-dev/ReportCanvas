namespace ReportCanvas.Application.Features.Auth.DTOs;

public record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    string OrganizationName
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserDto User
);

public record UserDto(
    string Id,
    string Email,
    string FullName
);
