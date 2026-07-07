using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportCanvas.Application.Common.Interfaces;
using ReportCanvas.Infrastructure.Parsing;
using ReportCanvas.Infrastructure.Persistence;
using ReportCanvas.Infrastructure.Storage;

namespace ReportCanvas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core + PostgreSQL
        services.AddDbContext<ReportCanvasDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ReportCanvasDbContext).Assembly.FullName)));

        // ASP.NET Core Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ReportCanvasDbContext>()
            .AddDefaultTokenProviders();

        // Storage
        services.AddScoped<IFileStorageService, AzureBlobStorageService>();

        // Dataset parsers — all parsers registered, resolved by file extension
        services.AddTransient<IDatasetParser, CsvDatasetParser>();
        services.AddTransient<IDatasetParser, ExcelDatasetParser>();

        return services;
    }
}
