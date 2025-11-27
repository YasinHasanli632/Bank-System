using FileStorageService.Application.DTOs;
using FileStorageService.Application.Services;
using FileStorageService.Domain.Entities;
using Moq;
using RedisLibrary.Interfaces;
using SharedLibrary.FileStorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Tests
{
    public class FileStorageServiceTests
    {
        private readonly Mock<IFileStorageRepository> _repoMock = new();
        private readonly Mock<IRabbitMQPublisher> _publisherMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly Application.Services.FileStorageService _service;

        public FileStorageServiceTests()
        {
            _service = new Application.Services.FileStorageService(_repoMock.Object, _publisherMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFiles()
        {
            
            _repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<FileStorage> { new FileStorage { FileId = 1, FileName = "test.pdf" } });

           
            var result = await _service.GetAllAsync();

           
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task AddAsync_ShouldPublishEvent()
        {
            
            var dto = new FileStorageCreateDto
            {
                CustomerId = 1,
                FileName = "file.pdf",
                FileExtension = ".pdf",
                MimeType = "application/pdf",
                FileSize = 123,
                FileCategory = "Doc",
                StoragePath = "/file",
                PublicUrl = "http://localhost/file.pdf"
            };

           
            await _service.AddAsync(dto);

           
            _repoMock.Verify(r => r.AddAsync(It.IsAny<FileStorage>()), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<FileStorageCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldPublishEventAndRemoveCache()
        {
           
            await _service.DeleteAsync(1);

           
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
            _publisherMock.Verify(p => p.PublishAsync(It.IsAny<FileStorageDeletedEvent>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
