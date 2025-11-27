using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Data;
using CustomerService.Infrastructure.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
namespace CustomerService.Tests
{
    public class CustomerRepositoryTests
    {
        private readonly CustomerRepository _repository;
        private readonly CustomerDbContext _context;

        public CustomerRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // hər test üçün yeni baza
                .EnableSensitiveDataLogging()
                .Options;

            _context = new CustomerDbContext(options);
            _repository = new CustomerRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Ali",
                LastName = "Həsənli",
                Email = "ali@test.com",
                PhoneNumber = "0501234567"
            };

            // Act
            await _repository.AddAsync(customer);
            var result = await _repository.GetByIdAsync(customer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ali", result.FirstName);
            Assert.Equal("0501234567", result.PhoneNumber);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Silinəcək",
                LastName = "Müştəri",
                Email = "delete@test.com",
                PhoneNumber = "0509998877"
            };

            await _repository.AddAsync(customer);

            // EF əlavə etdikdən sonra customer.Id artıq doludur
            var addedCustomer = await _repository.GetByIdAsync(customer.Id);
            Assert.NotNull(addedCustomer);

            // Act
            await _repository.DeleteAsync(customer.Id);
            var result = await _repository.GetByIdAsync(customer.Id);

            // Assert
            Assert.Null(result);
        }
    }

}
