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

        public DbSet<TrackerTask> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Email);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(50);

            modelBuilder.Entity<TrackerTask>().ToTable("Tasks");
            modelBuilder.Entity<TrackerTask>().HasKey(u => u.Id);
            modelBuilder.Entity<TrackerTask>().Property(u => u.PublicId).IsRequired();
            modelBuilder.Entity<TrackerTask>().HasIndex(u => u.PublicId).IsUnique();
            modelBuilder.Entity<TrackerTask>().Property(u => u.Title).IsRequired();

            modelBuilder.Entity<TrackerTask>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(f => f.AssignedToEmail);

            base.OnModelCreating(modelBuilder);
        }
    }
}