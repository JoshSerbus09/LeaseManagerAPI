using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using LeaseManagerAPI.Models;
using LeaseManagerAPI.Helpers;
using Microsoft.Extensions.Logging;
using LeaseManagerAPI.Data;
using System.Text;
using System.Globalization;

namespace LeaseManagerAPI.Controllers
{
    [ApiController]
    public class LeaseController : Controller
    {
        private readonly ILeaseDao _leaseDao;
        private readonly LeaseModelValidator _leaseValidotor;
        private readonly ILogger<LeaseController> _logger;
        public LeaseController(ILeaseDao leaseDao, LeaseModelValidator leaseValidator, ILogger<LeaseController> logger)
        {
            _leaseDao = leaseDao;
            _leaseValidotor = leaseValidator;
            _logger = logger;
        }

        [HttpGet("GetAllLeases")]
        public ActionResult GetLeases()
        {
            _logger.LogTrace($"attempting to get all leases.");

            try
            {
                var allLeases = _leaseDao.GetAllLeases();

                _logger.LogInformation($"successfully found {allLeases.Count} leases.");

                return Ok(allLeases);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"failed to get all leases.");
                return StatusCode(500, $"error while attempting to get all leases.");
            }
        }

        [HttpGet("ExportMonthlyPayments")]
        public ActionResult ExportMonthlyPayments([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate <= startDate)
            {
                _logger.LogError($"invalid date range requested for export. end date must be greater than start date.");
                return StatusCode(400, $"end date must be later than start date.");
            }

            try
            {
                var relevantLeases = _leaseDao.GetLeasesByDateRange(startDate, endDate);

                var csvBuilder = new StringBuilder();

                csvBuilder.Append(GlobalLeaseConstants.LEASE_EXPORT_HEADER_ROWS);
                csvBuilder.Append("\n");

                // calc & export
                for (var yx = startDate.Year; yx <= endDate.Year; yx++)
                {
                    for (var mx = startDate.Month; mx <= endDate.Month; mx++)
                    {
                        var monthlyLeasePayment = new decimal(0.0);
                        var monthlyInterestPayment = new decimal(0.0);

                        if (mx > 12)
                        {
                            // safety check :)
                            break;
                        }

                        foreach(var lease in relevantLeases)
                        {
                            if (LeaseCalcluationUtils.LeaseHasPaymentInMonth(yx, mx, lease))
                            {
                                monthlyLeasePayment += lease.PaymentAmount;

                                var baseRemainingPaymentAmount = LeaseCalcluationUtils.GetLeaseRemainingPayments(yx, mx, lease) * lease.PaymentAmount;

                                monthlyInterestPayment += decimal.Round(baseRemainingPaymentAmount * lease.InterestRate, 2);
                            }
                        }

                        csvBuilder.Append($"{yx}," +
                            $"{((MonthNameEnum)mx).ToString()}," +
                            $"{GetCurrencyString(monthlyLeasePayment)}," +
                            $"{GetCurrencyString(monthlyInterestPayment)}," +
                            $"{GetCurrencyString((monthlyLeasePayment + monthlyInterestPayment))}" +
                            $"\n");
                    }
                }

                return Ok(csvBuilder.ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"error while attempting to export leases.");
                return StatusCode(500, $"error while attempting to export leases.");
            }

            return Ok();
        }

        private string GetCurrencyString(decimal value)
        {
            return value.ToString("C", CultureInfo.GetCultureInfoByIetfLanguageTag("en-us"));
        }


        [HttpGet("GetLeaseById")]
        public ActionResult GetLeaseById([FromQuery] int id)
        {
            _logger.LogTrace($"attempting to get lease with id '{id}'.");

            try
            {
                var leaseRecord = _leaseDao.GetLeaseById(id);

                _logger.LogInformation($"found lease with id '{id}'.");

                return Ok(leaseRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"failed to get lease with id '{id}'.");
                return StatusCode(500, $"error while attempting to get lease with id '{id}'.");
            }
        }

        [HttpGet("GetLeasesByIds")]
        public ActionResult GetLeasesByIds([FromQuery] List<int> ids)
        {
            _logger.LogTrace($"attempting to get leases by ids [{string.Join(", ", ids.Select(id => $"{id}"))}]");

            var retrievedLeaseRecords = new List<BaseLeaseModel>();

            try
            {
                foreach(var id in ids)
                {
                    var leaseRecord = _leaseDao.GetLeaseById(id);

                    if (leaseRecord != null)
                    {
                        retrievedLeaseRecords.Add(leaseRecord);
                    }
                }

                _logger.LogInformation($"retrieved {retrievedLeaseRecords.Count} leases.");

                return Ok(retrievedLeaseRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"failed to get leases by ids");
                return StatusCode(500, $"error while attempting to get leases by ids");
            }
        }

        [HttpPost("CreateLease")]
        public ActionResult<BaseLeaseModel> Post([FromBody] BaseLeaseModel lease)
        {
            if (lease == null)
            {
                _logger.LogError($"invalid lease model request");
                return StatusCode(400, $"error while attempting to save lease.");
            }

            _logger.LogTrace($"attempting to save lease with name '{lease.Name}'.");

            try
            {
                if (_leaseValidotor.AreLeasePropertiesValid(lease))
                {
                    _logger.LogInformation($"lease properties are valid - creating lease.");
                    return Ok(_leaseDao.UpsertLease(lease));
                }

                _logger.LogInformation($"failed to create lease - invalid property information.");
                return StatusCode(500, $"could not create lease - lease properties not valid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"failed to save lease with name '{lease.Name}'.");
                return StatusCode(500, $"error while attempting to save lease with name '{lease.Name}'");
            }
        }

        [HttpPost("CreateLeases")]
        public ActionResult<List<BaseLeaseModel>> Post([FromBody] List<BaseLeaseModel> leases)
        {
            if (leases == null || leases.Count == 0)
            {
                _logger.LogError($"invalid lease model request");
                return StatusCode(400, $"error while attempting to save lease.");
            }

            if (leases.Any(lease => !_leaseValidotor.AreLeasePropertiesValid(lease)))
            {
                _logger.LogError($"request contained invalid leases");
                return StatusCode(400, $"lease properties invalid - no leases were imported");
            }

            try
            {
                _logger.LogInformation($"lease properties are valid - creating lease.");
                _leaseDao.UpsertLeases(leases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"failed to save leases.");
                return StatusCode(500, $"error while attempting to save leases'");
            }

            return Ok(leases);
        }

        [HttpPut("UpdateLease")]
        public ActionResult Put([FromBody] BaseLeaseModel lease)
        {
            if (lease == null)
            {
                _logger.LogError($"failed to update lease - invalid lease request.");
                return StatusCode(400, $"failed to update lease - invalid lease");
            }
            else if (lease.Id == null )
            {
                _logger.LogError($"failed to update lease - invalid lease request.");
                return StatusCode(404, $"failed to update lease - invalid lease.");
            }

            try
            {
                if (_leaseValidotor.AreLeasePropertiesValid(lease))
                {
                    return Ok(lease);
                }

                return StatusCode(404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error while updating lease with id '{lease.Id}'.");
            }

            return StatusCode(500, $"error while attempting to update lease with id '{lease.Id}'");
        }

        [HttpDelete("DeleteLease")]
        public ActionResult Delete([FromQuery] int id)
        {
            if (id < 0)
            {
                _logger.LogError($"failed to delete lease - invalid lease request.");
                return StatusCode(400, $"failed to delete lease - invalid lease.");
            }

            try
            {
                var leaseRecord = _leaseDao.GetLeaseById(id);

                if (leaseRecord == null)
                {
                    _logger.LogInformation($"unable to find lease with id '{id}'");
                    return StatusCode(404, $"unable to find lease with id '{id}'");
                }
                else
                {
                    _leaseDao.DeleteLease(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error while attempting to delete lease with id '{id}'.");
                return StatusCode(500, $"error while attempting to delete lease with id '{id}'.");
            }
            _logger.LogInformation($"lease with id '{id}' successfully deleted.");
            return Ok($"lease with id '{id}' was successfully deleted.");
        }
    }
}
