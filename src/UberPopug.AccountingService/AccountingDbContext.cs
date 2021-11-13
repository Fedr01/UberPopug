﻿using Microsoft.EntityFrameworkCore;
using UberPopug.AccountingService.Tasks;
using UberPopug.AccountingService.Users;

namespace UberPopug.AccountingService
{
    public class AccountingDbContext : DbContext
    {
        public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
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
            modelBuilder.Entity<TrackerTask>().Property(u => u.Title).IsRequired();

            modelBuilder.Entity<TrackerTask>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(f => f.AssignedToEmail);

            base.OnModelCreating(modelBuilder);
        }
    }
}