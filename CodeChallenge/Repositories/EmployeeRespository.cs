using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
        public ReportingStructure GetReportingStructureById(string id)
        {

            //select the employee record that matches the id passed in
            var parentEmployee = _employeeContext.Employees.Include(e => e.DirectReports).Where(e => e.EmployeeId == id).FirstOrDefault();
            //pass the selected parent employee into our method to recursively add reports to the reporting structure
            var reportCount = GetReportCount(parentEmployee);

            //return the created ReportingStructure object
            return new ReportingStructure
            {
                EmployeeId = parentEmployee.EmployeeId,
                NumberOfReports = reportCount
            };
        }
        private int GetReportCount(Employee employee)
        {
            //initialize our counter for the total reports in the reporting structure
            var reportCount = 0;
            //check to make sure that the employee passed in has direct reports
            if (employee.DirectReports != null && employee.DirectReports.Count > 0)
            {
                //iterate through the direct reports of the original employee
                foreach (Employee x in employee.DirectReports)
                {
                     //add to the counter for the reporting employee/child
                    reportCount++;
                    //get the full reporting employee/child object from the DB. 
                    //I did not like adding this second database call to this method, but the db was not already including child objects of the initial set of children, and I needed to make sure they were present.
                    var childEmployee = _employeeContext.Employees.Include(e => e.DirectReports).Where(e => e.EmployeeId == x.EmployeeId).FirstOrDefault();
                    //check the reporting employee/child for direct reports
                    if (childEmployee.DirectReports != null && childEmployee.DirectReports.Count > 0)
                    {
                        //if direct reports are present, recursively call this function with the current reporting employee/child
                        reportCount += GetReportCount(childEmployee);
                    }
                }
            }
            return reportCount;
        }
    }
}
