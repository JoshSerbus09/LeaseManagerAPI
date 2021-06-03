using LeaseManagerAPI.Models;
using System;
using System.Collections.Generic;

namespace LeaseManagerAPI.Data
{
    public interface ILeaseDao
    {
        List<BaseLeaseModel> GetAllLeases();

        List<BaseLeaseModel> GetLeasesByDateRange(DateTime startDate, DateTime endDate);

        BaseLeaseModel GetLeaseById(int id);

        BaseLeaseModel UpsertLease(BaseLeaseModel lease);

        List<BaseLeaseModel> UpsertLeases(List<BaseLeaseModel> leases);

        bool DeleteLease(int id);
    }
}
