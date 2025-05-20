using Microsoft.EntityFrameworkCore;

namespace UserManagementApi;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var hasher = new PasswordHasher();
        var adminPassword = hasher.HashPassword("Admin123");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.NewGuid(),
                Login = "Admin",
                Password = adminPassword,
                Name = "System",
                Gender = 2,
                Admin = true,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow
            }
        );
    }
}