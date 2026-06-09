using Abp.Demo.Booking;
using Abp.Demo.Bookings;
using Abp.Demo.Resources;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Abp.Demo.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class DemoDbContext :
    AbpDbContext<DemoDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Bookings.Booking> Bookings { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DemoDbContext(DbContextOptions<DemoDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        
        builder.Entity<Resource>(b =>
        {
            b.ToTable(DemoConsts.DbTablePrefix + "Resources", DemoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(BookingConsts.MaxNameLength);
            b.Property(x => x.Description).IsRequired().HasMaxLength(BookingConsts.MaxDescriptionLength);
            b.Property(x => x.Location).IsRequired().HasMaxLength(BookingConsts.MaxLocationLength);
            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.Capacity).IsRequired();

            b.HasIndex(x => x.Type);
            b.HasIndex(x => x.IsActive);
        });

        builder.Entity<Bookings.Booking>(b =>
        {
            b.ToTable(DemoConsts.DbTablePrefix + "Bookings", DemoConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ResourceId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.StartTime).IsRequired();
            b.Property(x => x.EndTime).IsRequired();
            b.Property(x => x.Purpose).IsRequired().HasMaxLength(BookingConsts.MaxPurposeLength);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.CancellationReason).HasMaxLength(BookingConsts.MaxCancellationReasonLength);

            b.HasIndex(x => new { x.ResourceId, x.StartTime, x.EndTime });
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Status);

            b.HasOne<Resource>().WithMany().HasForeignKey(x => x.ResourceId).IsRequired();
        });
    }
}
