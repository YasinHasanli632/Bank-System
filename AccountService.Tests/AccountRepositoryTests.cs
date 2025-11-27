using AccountService.Domain.Entities;
using AccountService.Infrastructure.Data;
using AccountService.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;

namespace AccountService.Tests
{
    public class AccountRepositoryTests
    {
        private readonly AccountDbContext _context;
        private readonly AccountRepository _repository;

        public AccountRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AccountDbContext(options);
            _repository = new AccountRepository(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Account()
        {
            var account = new Account
            {
                CustomerId = 1,
                AccountNumber = "ACC001",
                Balance = 100,
                Currency = "AZN",
                CreatedAt = DateTime.UtcNow
            };

            var added = await _repository.AddAsync(account);
            var result = await _repository.GetByIdAsync(added.AccountId);

            Assert.NotNull(result);
            Assert.Equal("ACC001", result.AccountNumber);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Account()
        {
            var account = new Account
            {
                CustomerId = 2,
                AccountNumber = "DEL001",
                Balance = 200,
                Currency = "USD",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(account);
            var deleted = await _repository.DeleteAsync(account.AccountId);

            Assert.True(deleted);
            var result = await _repository.GetByIdAsync(account.AccountId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCustomerIdAsync_Should_Return_Accounts()
        {
            await _repository.AddAsync(new Account
            {
                CustomerId = 99,
                AccountNumber = "CUST001",
                Balance = 400,
                Currency = "EUR",
                CreatedAt = DateTime.UtcNow
            });

            var list = await _repository.GetByCustomerIdAsync(99);
            Assert.Single(list);
        }
    }
}
