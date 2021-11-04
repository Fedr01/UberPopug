using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UberPopug.TaskTrackerService.Tasks;
using UberPopug.TaskTrackerService.Users;

namespace UberPopug.TaskTrackerService
{
    public class TaskTrackerDbContext : DbContext
    {
        public TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Email);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(50);

            modelBuilder.Entity<Task>().HasKey(u => u.Id);
            modelBuilder.Entity<Task>().Property(u => u.Description).IsRequired();

            modelBuilder.Entity<Task>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(f => f.UserEmail);

            base.OnModelCreating(modelBuilder);
        }
    }
}