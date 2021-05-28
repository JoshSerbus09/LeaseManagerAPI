using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Models
{
    public class LeaseModelOptions
    {
        public decimal MinimumLeasePaymentAmount { get; set; }
        public decimal MaximumLeasePaymentAmount { get; set; }
    }
}
