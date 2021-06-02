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
                && ArePaymentTermsValid(lease)
                && IsInterestRateValid(lease);
        }

        /// <summary>
        ///     The lease duration is a positive, non-zero DateTime length.
        /// </summary>
        private bool IsDateRangeValid(BaseLeaseModel lease)
        {
            if (lease.EndDate >= lease.StartDate)
            {
                _logger.LogInformation($"lease with name '{lease.Name}' has a valid date range");
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

            if (lease.NumPayments > 0 
                && GetMonthDurationOfLease(lease.StartDate, lease.EndDate) >= lease.NumPayments)
            {
                _logger.LogInformation($"lease with name '{lease.Name}' has a valid number of payments");
                return true;

            }
            _logger.LogError($"invalid number of payments {lease.Name}. number of payments should be less than or equal to the number of months in the lease.");

            return false; ;
        }

        private int GetMonthDurationOfLease(DateTime begin, DateTime end)
        {
            var startDateMonths = (begin.Year * 12) + begin.Month - 1; // subtract a month from the lease start for 0 index
            var endDateMonths = (end.Year * 12) + end.Month;

            return endDateMonths - startDateMonths;
        }

        /// <summary>
        ///     The lease payments are between the configured minimum and maximum amounts.
        /// </summary>
        private bool ArePaymentTermsValid(BaseLeaseModel lease)
        {
            
            if (lease.PaymentAmount > _leaseModelOptions.CurrentValue.MinimumLeasePaymentAmount
                && lease.PaymentAmount < _leaseModelOptions.CurrentValue.MaximumLeasePaymentAmount)
            {
                _logger.LogInformation($"lease with name '{lease.Name}' has a valid payment amount");
                return true;
            }

            return false;
        }

        /// <summary>
        ///     The lease interest rate is between 0 and configured max interest rate %
        /// </summary>
        /// <param name="lease"></param>
        /// <returns></returns>
        private bool IsInterestRateValid(BaseLeaseModel lease)
        {
            if (lease.InterestRate > 0 && lease.InterestRate <= _leaseModelOptions.CurrentValue.MaximumInterestRate)
            {
                _logger.LogInformation($"lease with name '{lease.Name}' has a valid interest rate");
                return true;
            }

            return false;
        }
    }
}
