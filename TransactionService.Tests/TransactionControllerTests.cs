using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.API.Controllers;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;

namespace TransactionService.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _serviceMock = new();
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _controller = new TransactionController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<TransactionReadDto> { new TransactionReadDto { TransactionId = 1 } });

            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenMissing()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((TransactionReadDto)null);

            var result = await _controller.GetById(5);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var dto = new TransactionCreateDto { AccountId = 1, TransactionType = "Deposit", Amount = 100 };
            _serviceMock.Setup(s => s.AddAsync(dto))
                .ReturnsAsync(new TransactionReadDto { TransactionId = 1 });

            var result = await _controller.Create(dto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeleted()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
