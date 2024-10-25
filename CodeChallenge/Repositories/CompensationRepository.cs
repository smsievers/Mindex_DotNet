using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;
using Microsoft.AspNetCore.Mvc;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }
        public Compensation CreateCompensation(Compensation compensation)
        {
            //perform the addition of the new compensation object to the database, and return it
            _employeeContext.Add(compensation);
            return compensation;
        }
        public Compensation GetCompensationByEmployeeId(String employeeId)
        {
            //query the database for the compensation object applicable to the employee with the id passed in, include the employee itself, and return the full object
            var compensation = _employeeContext.Compensations.Include(c => c.Employee).Include(c =>c.Employee.DirectReports).FirstOrDefault(c => c.Employee.EmployeeId == employeeId);
            return compensation;
        }
        public Task SaveAsync()
        {
            //save any currently pending changes to the database
            return _employeeContext.SaveChangesAsync();
        }
    }
}
