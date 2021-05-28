using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManager.SharedModels
{
    public interface ILeaseModel
    {
        int Id { get; set; }
        Guid? LeaseId { get; set; }
        
        string Name { get; set; }

        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        decimal PaymentAmount { get; set; }

        int NumPayments { get; set; }

        decimal InterestRate { get; set; }

        string Notes { get; set; }

        string LeaseJsonRaw { get; set; }

    }
}
