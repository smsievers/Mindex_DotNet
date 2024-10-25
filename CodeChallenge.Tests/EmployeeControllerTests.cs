
using System.Net;
using System.Net.Http;
using System.Text;
using CodeChallenge.Models;
using System.Collections.Generic;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructureById_Returns_Ok()
        {
            //Arrange
            //create our mock employee for testing, complete with a multi-layer reporting structure
            var employee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                FirstName = "John",
                LastName = "Lennon",
                Position = "Development Manager",
                Department = "Engineering",
                DirectReports = new List<Employee>
        {
            new Employee {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                FirstName = "Ringo",
                LastName = "Starr",
                Position = "Developer V",
                Department = "Engineering",
                DirectReports = new List<Employee>
                {
                    new Employee { EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309" },
                    new Employee { EmployeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c" }
                }
         },
            new Employee {
                    EmployeeId = "b7839309-3348-463b-a7e3-5de1c168beb3",
                    FirstName = "Paul",
                    LastName = "McCartney",
                    Position = "Developer I",
                    Department = "Engineering"
                }
        }
            };
            //create expected values for comparison with our returned result
            var expectedEmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedNumberOfReports = 4;

            //Execute
            //create and execute the get reeuest that will execute our logic for testing
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{employee.EmployeeId}");
            var response = getRequestTask.Result;

            //Assert
            //verify that the values of our result object match our expected values
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedEmployeeId, reportingStructure.EmployeeId);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.NumberOfReports);

        }

        [TestMethod]
        public void GetReportingStructureById_Returns_NotFoudn()
        {
            // Arrange
            //create empty employee object and convert to JSON payload
            var employee = new Employee()
            {
                EmployeeId = ""
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            //create and execute the get reeuest that will execute our logic for testing
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{employee.EmployeeId}");
            var response = getRequestTask.Result;


            //Assert
            //validate that no result is found when our method is executed with our empty string employeeId
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
