using CampusFix.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<RequestComment> RequestComments => Set<RequestComment>();
    public DbSet<RequestStatusHistory> RequestStatusHistory => Set<RequestStatusHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ServiceRequest>()
            .Property(r => r.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Entity<ServiceRequest>()
            .Property(r => r.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Entity<ServiceRequest>()
            .HasMany(r => r.Comments)
            .WithOne(c => c.ServiceRequest)
            .HasForeignKey(c => c.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ServiceRequest>()
            .HasMany(r => r.StatusHistory)
            .WithOne(h => h.ServiceRequest)
            .HasForeignKey(h => h.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
