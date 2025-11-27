using AccountService.API.Controllers;
using AccountService.Application.DTOs;
using AccountService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _serviceMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _serviceMock = new Mock<IAccountService>();
            _controller = new AccountController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithAccounts()
        {
            // Arrange
            var accounts = new List<GetAccountDto>
            {
                new GetAccountDto { AccountId = 1, AccountNumber = "ACC001" }
            };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(accounts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<GetAccountDto>>(okResult.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenAccountExists()
        {
            // Arrange
            var account = new GetAccountDto { AccountId = 1, AccountNumber = "ACC001" };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(account);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GetAccountDto>(okResult.Value);
            Assert.Equal(1, value.AccountId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((GetAccountDto)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var dto = new CreateAccountDto { CustomerId = 1, AccountNumber = "ACC100", Balance = 100, Currency = "AZN" };
            var created = new GetAccountDto { AccountId = 1, AccountNumber = "ACC100" };

            _serviceMock.Setup(s => s.AddAsync(dto)).ReturnsAsync(created);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
            var value = Assert.IsType<GetAccountDto>(createdAtActionResult.Value);
            Assert.Equal(1, value.AccountId);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // Arrange
            var dto = new UpdateAccountDto { AccountId = 1, AccountNumber = "NEW001", Balance = 200, Currency = "USD" };
            var updated = new GetAccountDto { AccountId = 1, AccountNumber = "NEW001" };

            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GetAccountDto>(okResult.Value);
            Assert.Equal("NEW001", value.AccountNumber);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var dto = new UpdateAccountDto { AccountId = 2, AccountNumber = "NEW001" };

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var dto = new UpdateAccountDto { AccountId = 1, AccountNumber = "NEW001" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync((GetAccountDto)null);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByCustomerId_ShouldReturnOk()
        {
            // Arrange
            var accounts = new List<GetAccountDto>
            {
                new GetAccountDto { AccountId = 1, CustomerId = 5, AccountNumber = "ACC005" }
            };

            _serviceMock.Setup(s => s.GetByCustomerIdAsync(5)).ReturnsAsync(accounts);

            // Act
            var result = await _controller.GetByCustomerId(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<GetAccountDto>>(okResult.Value);
            Assert.Single(value);
        }
    }
}
