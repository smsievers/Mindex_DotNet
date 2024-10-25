
using System.Net;
using System.Net.Http;
using System.Text;
using CodeChallenge.Models;
using System.Collections.Generic;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
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
        public void CreateCompensation_Returns_Ok()
        {
            // Arrange
            //create the test compensation object that will be used to verify our logic
            var compensation = new Compensation()
            {
                Employee = new Employee
                {
                    EmployeeId = "26a597ae-ede3-4847-99fe-c4518e82c86f",
                    FirstName = "Dave",
                    LastName = "Smith",
                    Position = "Development Henchman",
                    Department = "Engineering",
                    DirectReports = new List<Employee>()
                },
                Salary = 1235.67m,
                EffectiveDate = new DateTime(2012, 01, 01)
            };
            //Convert the compensation object into a JSON payload
            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            //execute our method logic using the test compensation created above, and record the response
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            //verify that the response status and data matches our expectations
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(compensation.Employee.EmployeeId, newCompensation.Employee.EmployeeId);
            Assert.AreEqual(compensation.Employee.FirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(compensation.Employee.LastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Employee.Department, newCompensation.Employee.Department);
            Assert.AreEqual(compensation.Employee.Position, newCompensation.Employee.Position);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            //create the employeeId string that we will be using to test our get method
            var employeeId = "16a597ae-ede3-4847-99fe-c4518e82c86f";

            //create the test compensation object that will temporarily stored to test against
            var expectedCompensation = new Compensation()
            {
                Employee = new Employee
                {
                    EmployeeId = "16a597ae-ede3-4847-99fe-c4518e82c86f",
                    FirstName = "John",
                    LastName = "Smith",
                    Position = "Development Henchman",
                    Department = "Engineering",
                    DirectReports = new List<Employee>()
                },
                Salary = 1235.67m,
                EffectiveDate = DateTime.Parse("2012-04-23T18:25:43.511Z").ToUniversalTime(),
            };

            // Execute
            //execute our method logic using the test compensation created above, and record the response
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;


            // Assert
            //verify that the response status and data matches our expectations
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var resultCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedCompensation.Employee.EmployeeId, resultCompensation.Employee.EmployeeId);
            Assert.AreEqual(expectedCompensation.Employee.FirstName, resultCompensation.Employee.FirstName);
            Assert.AreEqual(expectedCompensation.Employee.LastName, resultCompensation.Employee.LastName);
            Assert.AreEqual(expectedCompensation.Employee.Department, resultCompensation.Employee.Department);
            Assert.AreEqual(expectedCompensation.Employee.Position, resultCompensation.Employee.Position);
            Assert.AreEqual(expectedCompensation.Salary, resultCompensation.Salary);
            Assert.AreEqual(expectedCompensation.EffectiveDate, resultCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_NotFound()
        {
            // Arrange
            //create the employeeId string that we will be using to test our get method
            var employeeId = "this is a fake ID!";

            // Execute
            //execute our method logic using the test compensation created above, and record the response
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;


            // Assert
            //verify that the response status matches our expectations
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
