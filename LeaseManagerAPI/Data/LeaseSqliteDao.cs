using LeaseManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Data
{
    public class LeaseSqliteDao : ILeaseDao
    {
        private readonly LeaseSqliteDbContext _dbContext;

        public LeaseSqliteDao(LeaseSqliteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<BaseLeaseModel> GetAllLeases()
        {
            using (var context = _dbContext)
            {
                return context.Leases?.ToList();
            }
        }

        public BaseLeaseModel GetLeaseById(int id)
        {
            using (var context = _dbContext)
            {
                return context.Leases?.FirstOrDefault(lease => lease.Id == id);
            }
        }

        public BaseLeaseModel UpsertLease(BaseLeaseModel leaseToUpsert)
        {
            using (var context = _dbContext)
            {
                if (leaseToUpsert.Id != null)
                {
                    var existingLeaseRecord = context.Leases?.FirstOrDefault(l => l.Id == leaseToUpsert.Id);

                    if (existingLeaseRecord != null)
                    {
                        OverrideLeaseAttributes(existingLeaseRecord, leaseToUpsert);
                    }

                    context.SaveChanges();

                    return existingLeaseRecord;
                }
                else
                {
                    leaseToUpsert.Id = (context.Leases?.Count() + 1);
                    context.Leases.Add(leaseToUpsert);
                    context.SaveChanges();

                    return leaseToUpsert;
                } 
            }
        }

        public bool DeleteLease(int leaseId)
        {
            using (var context = _dbContext)
            {
                var leaseToRemove = context.Leases?.FirstOrDefault(l => l.Id == leaseId);

                if (leaseToRemove != null)
                {
                    context.Leases.Remove(leaseToRemove);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        private void OverrideLeaseAttributes(BaseLeaseModel lease1, BaseLeaseModel lease2)
        {
            if (lease1 == null || lease2 == null)
            {
                return;
            }

            lease1.Name = lease2.Name;
            lease1.StartDate = lease2.StartDate;
            lease1.EndDate = lease2.EndDate;
            lease1.PaymentAmount = lease2.PaymentAmount;
            lease1.NumPayments = lease2.NumPayments;
            lease1.InterestRate = lease2.InterestRate;
        }
    }
}
