using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LeaseManagerAPI.Models
{
    /// <summary>
    ///     Generic Lease Model. Could be converted to a base class or interface if additional lease constraints are introduced.
    /// </summary>
    public class BaseLeaseModel
    {
        [JsonProperty("Id")]
        public int? Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Start Date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("End Date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("Payment Amount")]
        public decimal PaymentAmount { get; set; }

        [JsonProperty("# of Payments")]
        public int NumPayments { get; set; }

        [JsonProperty("Interest Rate")]
        public decimal InterestRate { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }

    }
}
