using LautusInformatica.Models;
using Microsoft.EntityFrameworkCore;

namespace LautusInformatica.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<UsedItems> UsedItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users").HasKey(c => c.Id);
            modelBuilder.Entity<Item>().ToTable("Items").HasKey(i => i.Id);
            modelBuilder.Entity<ServiceOrder>().ToTable("ServiceOrders").HasKey(s => s.Id);
            modelBuilder.Entity<UsedItems>().ToTable("UsedItems").HasKey(u => u.Id);

            modelBuilder.Entity<ServiceOrder>()
                .HasOne(s => s.User)
                .WithMany(c => c.ServiceOrders)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UsedItems>()
                .HasOne(u => u.ServiceOrder)
                .WithMany(s => s.UsedItems)
                .HasForeignKey(u => u.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsedItems>()
                .HasOne(u => u.Item)
                .WithMany(i => i.UsedItems)
                .HasForeignKey(u => u.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasQueryFilter(c => c.IsDeleted == false);
            modelBuilder.Entity<Item>().HasQueryFilter(i => i.IsDeleted == false);
            modelBuilder.Entity<ServiceOrder>().HasQueryFilter(s => s.IsDeleted == false);
            modelBuilder.Entity<UsedItems>().HasQueryFilter(u => u.IsDeleted == false);
        }
    }
}
