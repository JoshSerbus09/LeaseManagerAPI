using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaseManagerAPI.Models;
using LeaseManagerAPI.Helpers;
using Microsoft.Extensions.Logging;
using LeaseManagerAPI.Data;

namespace LeaseManagerAPI.Controllers
{
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

        [HttpGet]
        [Route("GetAllLeases")]
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

        [HttpGet]
        [Route("Get")]
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
            return StatusCode(201, id);
        }

        [HttpGet]
        [Route("Get/{ids}")]
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

        [HttpPost]
        [Route("Post")]
        public ActionResult Post([FromBody] BaseLeaseModel lease)
        {
            if (lease == null)
            {
                _logger.LogError($"invalid lease model request");
                return StatusCode(400, $"error while attempting to save lease.");
            }

            _logger.LogTrace($"attempting to save lease with name '{lease.Name}'.");

            try
            {
                return Ok(_leaseDao.UpsertLease(lease));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"failed to save lease with name '{lease.Name}'.");
                return StatusCode(500, $"error while attempting to save lease with name '{lease.Name}'");
            }
        }

        [HttpPut]
        [Route("Put")]
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
            
            // TODO - check if record exists

            if (_leaseValidotor.AreLeasePropertiesValid(lease))
            {
                // upsert and return model
                return Ok(lease); 
            }
           

            return StatusCode(201);
        }

        [HttpDelete]
        [Route("Delete/{leaseId}")]
        public ActionResult Delete([FromQuery] int id)
        {
            if (id == null || id < 0)
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
                return StatusCode(500, $"error while attempting to delete lease with id '{id}'.");
            }
            _logger.LogInformation($"lease with id '{id}' successfully deleted.");
            return Ok($"lease with id '{id}' was successfully deleted.");
        }
    }
}
