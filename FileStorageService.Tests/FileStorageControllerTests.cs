using FileStorageService.API.Controllers;
using FileStorageService.Application.DTOs;
using FileStorageService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Tests
{
    public class FileStorageControllerTests
    {
        private readonly Mock<IFileStorageService> _serviceMock = new();
        private readonly FileStorageController _controller;

        public FileStorageControllerTests()
        {
            _controller = new FileStorageController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
           
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<FileStorageReadDto> { new FileStorageReadDto { FileId = 1 } });

           
            var result = await _controller.GetAll();

           
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenFileMissing()
        {
           
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((FileStorageReadDto)null);

           
            var result = await _controller.Get(5);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            
            var dto = new FileStorageCreateDto
            {
                CustomerId = 1,
                FileName = "test.pdf"
            };

           
            var result = await _controller.Create(dto);

           
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
           
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new FileStorageReadDto());

           
            var result = await _controller.Delete(1);

            
            Assert.IsType<NoContentResult>(result);
        }
    }
}
