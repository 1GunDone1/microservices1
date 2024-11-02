using Microsoft.EntityFrameworkCore;
using UserManagementApi.Models;

namespace UserManagementApi.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed данные
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "user1", Password = "pass1", IsOnline = false },
                new User { Id = 2, Username = "user2", Password = "pass2", IsOnline = false }
            );
        }
    }
}