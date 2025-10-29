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

        // 1️⃣ DbSets para todas as suas entidades
        public DbSet<Client> Clients { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<UsedItems> UsedItems { get; set; }

        // 2️⃣ Configurações adicionais
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>().ToTable("Clients").HasKey(c => c.Id);
            modelBuilder.Entity<Item>().ToTable("Items").HasKey(i => i.Id);
            modelBuilder.Entity<ServiceOrder>().ToTable("ServiceOrders").HasKey(s => s.Id);
            modelBuilder.Entity<UsedItems>().ToTable("UsedItems").HasKey(u => u.Id);

            modelBuilder.Entity<ServiceOrder>()
                .HasOne(s => s.Client)
                .WithMany(c => c.ServiceOrders)
                .HasForeignKey(s => s.ClientId)
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

            modelBuilder.Entity<Client>().HasQueryFilter(c => c.IsDeleted == false);
            modelBuilder.Entity<Item>().HasQueryFilter(i => i.IsDeleted == false);
            modelBuilder.Entity<ServiceOrder>().HasQueryFilter(s => s.IsDeleted == false);
            modelBuilder.Entity<UsedItems>().HasQueryFilter(u => u.IsDeleted == false);
        }
    }
}
