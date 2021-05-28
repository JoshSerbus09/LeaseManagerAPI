using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LeaseManagerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LeaseManagerAPI.Data
{
    public abstract class LeaseSqliteDbContext : DbContext
    {
        public readonly Func<IDbConnection> _dbConnection;
        public DbSet<BaseLeaseModel> Leases { get; set; }

        protected LeaseSqliteDbContext(Func<IDbConnection> dbConnection)
        {
            _dbConnection = dbConnection;
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
