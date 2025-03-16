using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using ModelLayer.model;

namespace RepositoryLayer.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AddressBookEntry> AddressBookEntries { get; set; } = null!;

        // Add a parameterless constructor for EF Core CLI tools
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressBookEntry>()
                .HasOne(ab => ab.User)
                .WithMany(u => u.AddressBookEntries)
                .HasForeignKey(ab => ab.UserId);
        }
    }
}
