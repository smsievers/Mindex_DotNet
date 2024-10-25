using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {

        Compensation GetCompensationByEmployeeId(String employeeId);
        Compensation CreateCompensation(Compensation compensation);
        Task SaveAsync();
    }
}
