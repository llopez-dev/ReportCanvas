namespace ReportCanvas.Application.Common.Interfaces;

/// <summary>Provides the currently authenticated user's context to the application layer.</summary>
public interface ICurrentUserService
{
    string UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}
