using Microsoft.Extensions.DependencyInjection;

namespace ReportCanvas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // FluentValidation scanners and other application-level registrations go here.
        // Validators will be added per-feature as they're implemented.
        return services;
    }
}
