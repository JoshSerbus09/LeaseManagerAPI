using LeaseManagerAPI.Models;
using System.Collections.Generic;

namespace LeaseManagerAPI.Data
{
    public interface ILeaseDao
    {
        List<BaseLeaseModel> GetAllLeases();

        BaseLeaseModel GetLeaseById(int id);

        BaseLeaseModel UpsertLease(BaseLeaseModel lease);

        List<BaseLeaseModel> UpsertLeases(List<BaseLeaseModel> leases);

        bool DeleteLease(int id);
    }
}
