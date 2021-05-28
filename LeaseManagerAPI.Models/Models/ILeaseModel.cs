using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Models
{
    public interface ILeaseModel
    {
        string Name { get; set; }

        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        Decimal PaymentAmount { get; set; }

        int NumPayments { get; set; }

        decimal InterestRate { get; set; }

        string Notes { get; set; }

    }
}
