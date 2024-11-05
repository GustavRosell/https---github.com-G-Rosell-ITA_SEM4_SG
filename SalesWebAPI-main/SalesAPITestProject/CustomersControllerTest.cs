using Microsoft.AspNetCore.Mvc;

using SalesWebAPI.Controllers;
using SalesWebAPI.Interfaces;
using SalesWebAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace SalesAPITestProject
{
    [TestClass]
    public class CustomersControllerTest
    {
        [TestMethod]
        public void GetCustomers_ShouldReturnLists()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();
            mockRepo.Setup(repo => repo.GetCustomers()).Returns(GetTestCustomers());

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var customers = okResult.Value as List<Customer>;
            Assert.IsNotNull(customers);
            Assert.AreEqual(2, customers.Count);
        }

        [TestMethod]
        public void GetCustomers_When_Called_returnsNull()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();
            mockRepo.Setup(repo => repo.GetCustomers()).Returns(() => null);

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNull(okResult.Value);
        }

        [TestMethod]
        public void GetCustomer_WithAnInvalidId_ShouldReturnNotFound()
        {
        }


        [TestMethod]
        public void GetCustomer_WithAValidId_ShouldReturnCustomer()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();
            mockRepo.Setup(repo => repo.GetById(1)).Returns(new Customer { CustomerId = 1, Name = "John Doe" });

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var customer = okResult.Value as Customer;
            Assert.IsNotNull(customer);
            Assert.AreEqual(1, customer.CustomerId);
            Assert.AreEqual("John Doe", customer.Name);
        }

        private List<Customer> GetTestCustomers()
        {
            return new List<Customer>
            {
                new Customer { CustomerId = 1, Name = "John Doe" },
                new Customer { CustomerId = 2, Name = "Jane Doe" }
            };
        }
    }
}