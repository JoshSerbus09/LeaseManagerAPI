using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Models
{
    public class LeaseModelOptions
    {
        public string CurrencyLocaleIetString { get; set; }
        public decimal MinimumLeasePaymentAmount { get; set; }
        public decimal MaximumLeasePaymentAmount { get; set; }
        public decimal MaximumInterestRate { get; set; }
    }
}
