using CustomerService.API.Controllers;
using CustomerService.Application.DTOs;
using CustomerService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _serviceMock;
        private readonly CustomerController _controller;

        public CustomersControllerTests()
        {
            _serviceMock = new Mock<ICustomerService>();
            _controller = new CustomerController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<CustomerDto> { new() { Id = 1, FirstName = "Test" } });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var customers = Assert.IsAssignableFrom<IEnumerable<CustomerDto>>(okResult.Value);
            Assert.Single(customers);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((CustomerDto?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenEmailExists()
        {
            // Arrange
            var dto = new CreateCustomerDto { Email = "test@test.com" };
            _serviceMock.Setup(s => s.CustomerExistsAsync(dto.Email)).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Contains("exists", conflict.Value.ToString());
        }
    }
}
