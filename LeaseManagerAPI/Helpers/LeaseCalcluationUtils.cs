using LeaseManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaseManagerAPI.Helpers
{
    public static class LeaseCalcluationUtils
    {
        /// <summary>
        ///     Check if a lease has a scheduled payment in the target epoch
        /// </summary>
        /// <param name="year">target year</param>
        /// <param name="month">target month</param>
        /// <param name="lease">relevant lease</param>
        /// <returns></returns>
        public static bool LeaseHasPaymentInMonth(int year, int month, BaseLeaseModel lease)
        {
            var monthDurationIndex = GetEpochIndex(year, month);

            var leaseMonthBeginIndex = GetEpochIndex(lease.StartDate.Year, lease.StartDate.Month);
            var leasePaymentEndIndex = GetEpochIndex(lease.EndDate.Year, lease.EndDate.Month);

            if (monthDurationIndex > leaseMonthBeginIndex && monthDurationIndex <= leasePaymentEndIndex)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Subtract the elapsed months since the start of the lease from the total number of payments 
        /// </summary>
        /// <param name="year">target year</param>
        /// <param name="month">target month</param>
        /// <param name="lease">relevant lease</param>
        /// <returns></returns>
        public static int GetLeaseRemainingPayments(int year, int month, BaseLeaseModel lease)
        {
            var leaseStartMonthIndex = GetEpochIndex(year, month);

            var elapsedPayments = GetEpochIndex(year, month) - leaseStartMonthIndex;

            return lease.NumPayments - elapsedPayments;
        }

        private static int GetEpochIndex(int year, int month)
        {
            return (year * 12) + month;
        }
    }
}
