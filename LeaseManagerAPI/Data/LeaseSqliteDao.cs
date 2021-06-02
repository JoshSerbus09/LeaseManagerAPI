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
            return _dbContext.Leases?.OrderBy(lease => lease.StartDate).ToList();
        }

        public BaseLeaseModel GetLeaseById(int id)
        {
            return _dbContext.Leases?.FirstOrDefault(lease => lease.Id == id);
        }

        public BaseLeaseModel UpsertLease(BaseLeaseModel leaseToUpsert)
        {
            if (leaseToUpsert.Id > 0)
            {
                var existingLeaseRecord = _dbContext.Leases?.FirstOrDefault(l => l.Id == leaseToUpsert.Id);

                if (existingLeaseRecord != null)
                {
                    OverrideLeaseAttributes(existingLeaseRecord, leaseToUpsert);
                }

                _dbContext.SaveChanges();

                return existingLeaseRecord;
            }
            else
            {
                _dbContext.Leases.Add(leaseToUpsert);
                _dbContext.SaveChanges();

                return leaseToUpsert;
            }
        }

        public List<BaseLeaseModel> UpsertLeases(List<BaseLeaseModel> leases)
        {
            var savedLeases = new List<BaseLeaseModel>();
            foreach (var lease in leases)
            {
                if (lease.Id > 0)
                {
                    var existingLeaseRecord = _dbContext.Leases?.FirstOrDefault(l => l.Id == lease.Id);

                    if (existingLeaseRecord != null)
                    {
                        OverrideLeaseAttributes(existingLeaseRecord, lease);
                    }

                    _dbContext.SaveChanges();
                }
                else
                {
                    _dbContext.Leases.Add(lease);
                    _dbContext.SaveChanges();

                    savedLeases.Add(lease);

                }
            }

            return leases;
        }

        public bool DeleteLease(int leaseId)
        {
            if (leaseId < 0)
            {
                return false;
            }

            var leaseToRemove = _dbContext.Leases?.FirstOrDefault(l => l.Id == leaseId);

            if (leaseToRemove != null)
            {
                _dbContext.Leases.Remove(leaseToRemove);
                _dbContext.SaveChanges();
                return true;
            }

            return false;
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
