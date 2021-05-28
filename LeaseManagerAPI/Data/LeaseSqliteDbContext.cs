using System;
using System.Data;
using LeaseManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaseManagerAPI.Data
{
    public class LeaseSqliteDbContext : DbContext
    {
        public DbSet<BaseLeaseModel> Leases { get; set; }

        public LeaseSqliteDbContext()
        {   
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=LeasesDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseLeaseModel>()
                .Property(lease => lease.InterestRate)
                .HasConversion<decimal>()
                ;
            modelBuilder.Entity<BaseLeaseModel>()
                .Property(lease => lease.PaymentAmount)
                .HasConversion<decimal>();

            modelBuilder.Entity<BaseLeaseModel>().ToTable("Leases");
        }
    }
}
