using LeaseManagerAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Helpers
{
    public class LeaseModelValidator
    {
        private readonly IOptionsMonitor<LeaseModelOptions> _leaseModelOptions;
        private readonly ILogger<LeaseModelValidator> _logger;

        public LeaseModelValidator(IOptionsMonitor<LeaseModelOptions> leaseModelOptions, ILogger<LeaseModelValidator> logger)
        {
            _leaseModelOptions = leaseModelOptions;
            _logger = logger;
        }

        /// <summary>
        ///     Perform validation of the lease model.
        /// </summary>
        public bool AreLeasePropertiesValid(BaseLeaseModel lease)
        {
            _logger.LogTrace($"Validating lease with name {lease.Name}");

            return IsDateRangeValid(lease)
                && IsPaymentScheduleValid(lease)
                && ArePaymentTermsValid(lease);
        }

        /// <summary>
        ///     The lease duration is a positive, non-zero DateTime length.
        /// </summary>
        private bool IsDateRangeValid(BaseLeaseModel lease)
        {
            if (lease.EndDate > lease.StartDate)
            {
                return true;
            }

            _logger.LogError($"invalid date range for lease with name {lease.Name}. lease end date must be later than the start date");
            return false;
        }

        /// <summary>
        ///     The lease duration is greater than or equal to the number of lease payments.
        /// </summary>
        private bool IsPaymentScheduleValid(BaseLeaseModel lease)
        {
            return (lease.EndDate.Month - lease.StartDate.Month) >= lease.NumPayments;
        }

        /// <summary>
        ///     The lease payments are between the configured minimum and maximum amounts.
        /// </summary>
        private bool ArePaymentTermsValid(BaseLeaseModel lease)
        {
            return lease.PaymentAmount > _leaseModelOptions.CurrentValue.MinimumLeasePaymentAmount
                && lease.PaymentAmount < _leaseModelOptions.CurrentValue.MaximumLeasePaymentAmount;
        }
    }
}
