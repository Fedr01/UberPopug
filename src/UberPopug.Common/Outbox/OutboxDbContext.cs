using Microsoft.EntityFrameworkCore;

namespace UberPopug.Common.Outbox
{
    public class OutboxDbContext : DbContext
    {
        public OutboxDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxLetter>(e => e.ToTable("Outbox"));
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<OutboxLetter> Letters { get; set; }
    }
}