using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<ApplicationUser>()
            .HasMany(user => user.UserClaims)
            .WithOne()
            .HasForeignKey(claim => claim.UserId);

        builder
            .Entity<ApplicationUser>()
            .HasMany(user => user.Roles)
            .WithMany(role => role.Users)
            .UsingEntity<IdentityUserRole<string>>(
                builder => builder.HasOne<ApplicationRole>().WithMany().HasForeignKey(userRole => userRole.RoleId),
                builder => builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(userRole => userRole.UserId),
                builder => builder.ToTable("AspNetUserRoles")
            );

        builder
            .Entity<ApplicationRole>()
            .HasMany(role => role.RoleClaims)
            .WithOne()
            .HasForeignKey(claim => claim.RoleId);
    }
}