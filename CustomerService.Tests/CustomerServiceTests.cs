using CustomerService.Application.DTOs;
using CustomerService.Application.Interfaces;
using CustomerService.Application.Services;
using CustomerService.Domain.Entities;
using Moq;
using RedisLibrary.Interfaces;
using SharedLibrary.CustomerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repoMock = new();
        private readonly Mock<IRabbitMQPublisher> _rabbitMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly CustomerService.Application.Services.CustomerService _service;

        public CustomerServiceTests()
        {
            _service = new CustomerService.Application.Services.CustomerService(
                _repoMock.Object, _rabbitMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromCache_IfAvailable()
        {
            // Arrange
            var cached = new List<CustomerDto> { new() { Id = 1, FirstName = "Cache" } };
            _cacheMock.Setup(c => c.GetAsync<IEnumerable<CustomerDto>>("Customer_All"))
                      .ReturnsAsync(cached);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal("Cache", result.First().FirstName);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_PublishesEvent_AndClearsCache()
        {
            // Arrange
            var dto = new CreateCustomerDto { FirstName = "John", LastName = "Doe", Email = "john@test.com" };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
            _rabbitMock.Verify(r => r.PublishAsync(It.IsAny<CustomerCreatedEvent>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync("Customer_All"), Times.Once);
        }
    }
}

