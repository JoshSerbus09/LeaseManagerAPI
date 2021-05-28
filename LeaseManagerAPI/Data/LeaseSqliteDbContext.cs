using System;
using System.Data;
using LeaseManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaseManagerAPI.Data
{
    public class LeaseSqliteDbContext : DbContext
    {
        public DbSet<BaseLeaseModel> Leases { get; set; }

        private static bool _created = false;

        public LeaseSqliteDbContext()
        {
            if (!_created)
            {
                _created = true;
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=LeasesDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseLeaseModel>().ToTable("Leases");

            modelBuilder.Entity<BaseLeaseModel>(e =>
            {
                e.Property(lease => lease.InterestRate).HasConversion<double>();
                e.Property(lease => lease.PaymentAmount).HasConversion<double>();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
