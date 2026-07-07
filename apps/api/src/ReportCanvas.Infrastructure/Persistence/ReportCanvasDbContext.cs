using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReportCanvas.Domain.Entities;

namespace ReportCanvas.Infrastructure.Persistence;

/// <summary>
/// ApplicationUser extends IdentityUser to add domain-level properties.
/// We keep Identity concerns here rather than in Domain to avoid a framework dependency in Domain.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ReportCanvasDbContext : IdentityDbContext<ApplicationUser>
{
    public ReportCanvasDbContext(DbContextOptions<ReportCanvasDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Dataset> Datasets => Set<Dataset>();
    public DbSet<DatasetColumn> DatasetColumns => Set<DatasetColumn>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportPage> ReportPages => Set<ReportPage>();
    public DbSet<Widget> Widgets => Set<Widget>();
    public DbSet<BrandKit> BrandKits => Set<BrandKit>();
    public DbSet<FileAsset> FileAssets => Set<FileAsset>();
    public DbSet<ExportJob> ExportJobs => Set<ExportJob>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all entity type configurations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ReportCanvasDbContext).Assembly);

        // Rename Identity tables to match project conventions
        builder.Entity<ApplicationUser>().ToTable("users");
        builder.Entity<IdentityRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-update UpdatedAt on every save
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
