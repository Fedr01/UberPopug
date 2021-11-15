using Microsoft.EntityFrameworkCore;
using UberPopug.AccountingService.Tasks;
using UberPopug.AccountingService.Users;
using Transaction = UberPopug.AccountingService.Accounting.Transaction;

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
        public DbSet<Transaction> Transactions { get; set; }

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


            modelBuilder.Entity<Transaction>().ToTable("Transactions");
            modelBuilder.Entity<Transaction>().HasKey(u => u.Id);
            modelBuilder.Entity<Transaction>().Property(u => u.PublicId).IsRequired();
            modelBuilder.Entity<Transaction>().HasIndex(u => u.PublicId).IsUnique();
            modelBuilder.Entity<Transaction>().Property(u => u.Amount).IsRequired();
            modelBuilder.Entity<Transaction>().Property(u => u.UserEmail).IsRequired();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(f => f.UserEmail);

            base.OnModelCreating(modelBuilder);
        }
    }
}