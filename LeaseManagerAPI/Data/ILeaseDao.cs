using LeaseManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Data
{
    public interface ILeaseDao
    {
        List<BaseLeaseModel> GetAllLeases();

        BaseLeaseModel GetLeaseById(int id);

        BaseLeaseModel UpsertLease(BaseLeaseModel lease);

        bool DeleteLease(int id);
    }
}
