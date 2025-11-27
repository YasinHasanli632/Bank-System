using AccountService.Infrastructure.RabbitMQ.Interfaces;
using AccountService.Infrastructure.Repositories.Interface;
using Moq;
using RedisLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountService.Application.Services;
using AccountService.Application.DTOs;
using SharedLibrary.AccountService;
using AccountService.Domain.Entities;
namespace AccountService.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _repoMock;
        private readonly Mock<IRabbitMQPublisher> _publisherMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly AccountService.Application.Services.AccountService _service;

        public AccountServiceTests()
        {
            _repoMock = new Mock<IAccountRepository>();
            _publisherMock = new Mock<IRabbitMQPublisher>();
            _cacheMock = new Mock<ICacheService>();

            _service = new AccountService.Application.Services.AccountService(_repoMock.Object, _publisherMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task AddAsync_Should_Add_New_Account_And_Clear_Cache_And_Publish_Event()
        {
            // Arrange
            var dto = new CreateAccountDto
            {
                CustomerId = 1,
                AccountNumber = "ACC123",
                Balance = 1000,
                Currency = "AZN"
            };

            var entity = new Account
            {
                AccountId = 1,
                CustomerId = dto.CustomerId,
                AccountNumber = dto.AccountNumber,
                Balance = dto.Balance,
                Currency = dto.Currency,
                CreatedAt = DateTime.UtcNow
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Account>())).ReturnsAsync(entity);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.Equal(dto.CustomerId, result.CustomerId);
            Assert.Equal(dto.AccountNumber, result.AccountNumber);
            Assert.Equal(dto.Balance, result.Balance);

            _cacheMock.Verify(c => c.RemoveAsync("accounts_all"), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync("account_created_queue", It.IsAny<AccountCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_From_Repository_When_Not_In_Cache()
        {
            // Arrange
            var account = new Account
            {
                AccountId = 1,
                CustomerId = 2,
                AccountNumber = "ACC001",
                Balance = 500,
                Currency = "USD",
                CreatedAt = DateTime.UtcNow
            };

            _cacheMock.Setup(c => c.GetAsync<Account>("account_1")).ReturnsAsync((Account)null);
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ACC001", result.AccountNumber);
            _cacheMock.Verify(c => c.SetAsync("account_1", account, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Account_And_Publish_Event()
        {
            // Arrange
            var existing = new Account
            {
                AccountId = 1,
                AccountNumber = "OLD001",
                Balance = 200,
                Currency = "AZN"
            };

            var dto = new UpdateAccountDto
            {
                AccountId = 1,
                AccountNumber = "NEW001",
                Balance = 300,
                Currency = "USD"
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Account>())).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.AccountNumber, result.AccountNumber);
            Assert.Equal(dto.Currency, result.Currency);

            _cacheMock.Verify(c => c.RemoveAsync("accounts_all"), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync("account_1"), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync("account_updated_queue", It.IsAny<AccountUpdatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Account_And_Publish_Event()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _cacheMock.Verify(c => c.RemoveAsync("accounts_all"), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync("account_1"), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync("account_deleted_queue", It.IsAny<AccountDeletedEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_From_Cache_If_Available()
        {
            // Arrange
            var list = new List<Account>
            {
                new Account { AccountId = 1, AccountNumber = "A1", Balance = 100, Currency = "AZN" }
            };

            _cacheMock.Setup(c => c.GetAsync<IEnumerable<Account>>("accounts_all")).ReturnsAsync(list);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("A1", result.First().AccountNumber);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Never);
        }
    }
}
