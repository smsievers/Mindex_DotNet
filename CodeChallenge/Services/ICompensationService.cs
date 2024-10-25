using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {

        Compensation GetCompensationByEmployeeId(String employeeId);
        Compensation CreateCompensation(Compensation compensation);
    }
}
