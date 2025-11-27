using Moq;
using RedisLibrary.Interfaces;
using SharedLibrary.TransactionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs;
using TransactionService.Application.Services;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Repositories.Interfaces;

namespace TransactionService.Tests
{
    public  class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _repoMock = new();
        private readonly Mock<IRabbitMQPublisher> _publisherMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly Application.Services.TransactionService _service;

        public TransactionServiceTests()
        {
            _service = new  Application.Services.TransactionService  (_repoMock.Object, _publisherMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnTransactions()
        {
            _repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Transaction> { new Transaction { TransactionId = 1, Amount = 100 } });

            var result = await _service.GetAllAsync();

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task AddAsync_ShouldPublishEvent()
        {
            var dto = new TransactionCreateDto
            {
                AccountId = 1,
                TransactionType = "Deposit",
                Amount = 100,
                Currency = "USD"
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                     .ReturnsAsync(new Transaction { TransactionId = 1, AccountId = 1, Amount = 100 });

            var created = await _service.AddAsync(dto);

            Assert.Equal(1, created.TransactionId);
            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<TransactionRecordedEvent>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrueAndPublishEvent()
        {
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<TransactionDeletedEvent>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
