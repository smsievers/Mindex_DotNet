using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {

        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;
        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Compensation CreateCompensation(Compensation compensation)
        {
            //validate that a compensation object has been passed into the method
            if (compensation != null)
            {
                //if a compensation object has been passed in, call the repository methods for creating and saving the new compensation object to the data layer
                compensation = _compensationRepository.CreateCompensation(compensation);
                _compensationRepository.SaveAsync().Wait();
            }
            return compensation;
        }
        public Compensation GetCompensationByEmployeeId(String employeeId)
        {

            //validate that an employee Id has been passed in, and is not null or an empty string
            if (!String.IsNullOrEmpty(employeeId))
            {
                //if a compensation object has been passed in, call the repository methods for retrieving the compensation object from the data layer
                return _compensationRepository.GetCompensationByEmployeeId(employeeId);
            }
            return null;
        }
    }
}
