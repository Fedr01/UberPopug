using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UberPopug.AuthService.Users;

namespace UberPopug.AuthService
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Email);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();

            modelBuilder.Entity<User>().HasData(new List<User>
            {
                new() { Email = "admin@popug.ru", Password = "123", Role = Role.Admin },
                new() { Email = "manager@popug.ru", Password = "123", Role = Role.Manager },
                new() { Email = "accountant@popug.ru", Password = "123", Role = Role.Accountant },
                new() { Email = "emp1@popug.ru", Password = "123", Role = Role.Employee },
                new() { Email = "emp2@popug.ru", Password = "123", Role = Role.Employee },
                new() { Email = "emp3@popug.ru", Password = "123", Role = Role.Employee }
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}