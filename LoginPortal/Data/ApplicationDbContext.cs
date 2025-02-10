using Microsoft.EntityFrameworkCore;
using LoginPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LoginPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Inherit from IdentityDbContext with ApplicationUser
    {
        // Constructor passing options to the base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for each table in your database
        public DbSet<User_Details> UserDetails { get; set; }
        public DbSet<VerificationTokens> VerificationTokens { get; set; }
        public DbSet<AccessTokens> AccessTokens { get; set; }
        public DbSet<PasswordsResult> PasswordsResult { get; set; }

        public async Task<bool> CanConnectAsync()
        {
            try
            {
                return await Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }
        // Override OnModelCreating to configure the model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PasswordsResult is a view, so it has no primary key
            modelBuilder.Entity<PasswordsResult>().HasNoKey();

            // Let EF Core know the table has triggers
            modelBuilder.Entity<User_Details>()
                .ToTable("userDetails", t => t.HasTrigger("after_password_update"));

            // Add additional configuration if needed
            base.OnModelCreating(modelBuilder); // Ensure base configuration is called (important for Identity)

            // Further custom configurations for other entities can be added here
        }
    }
}
