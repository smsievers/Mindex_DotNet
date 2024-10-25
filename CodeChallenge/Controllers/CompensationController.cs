using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : Controller
    {

        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for employee '{compensation.Employee.EmployeeId}'");
            //call compensation service to create a compensation object using the request body
             compensation = _compensationService.CreateCompensation(compensation);
            //return CreatedAtRoute result with the information relevant to the created compensation object
            return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId}, compensation);
        }

        [HttpGet("{id}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            _logger.LogDebug($"Received compensation get request for employee '{id}'");

            //call employee service to get compensation object for the given id
            var compensation = _compensationService.GetCompensationByEmployeeId(id);

            //check to make sure that the result of the call is not null, and return NotFound() if it is
            if (compensation == null)
                return NotFound();

            //return Ok with the valid Compensation Result
            return Ok(compensation);
        }
    }

}
