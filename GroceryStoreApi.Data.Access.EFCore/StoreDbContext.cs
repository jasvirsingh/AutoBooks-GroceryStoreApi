using GroceryStore.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace GroceryStoreApi.Data.Access.EFCore
{
    public partial class StoreDbContext : DbContext
    {
        public virtual DbSet<CustomerEntity> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.\SQLExpress;Database=GroceryStore;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerEntity>(entity =>
            {
                entity.HasKey("Id");
                entity.ToTable("Customer");

                entity.Property(e => e.Id).HasColumnName("CustomerId");
                entity.Property(e => e.Name).HasColumnName("Name");
            });
        }
    }
}
