using DocumentService.Application.DTOs;
using DocumentService.Domain.Entities;
using DocumentService.Infrastructure.RabbitMQ.Interfaces;
using DocumentService.Infrastructure.Repositories.Interface;
using Moq;
using RedisLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Tests
{
    public class DocumentServiceTests
    {
        private readonly Mock<IDocumentRepository> _repoMock;
        private readonly Mock<IRabbitMQPublisher> _rabbitMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly DocumentService.Application.Services.DocumentService _service;

        public DocumentServiceTests()
        {
            _repoMock = new Mock<IDocumentRepository>();
            _rabbitMock = new Mock<IRabbitMQPublisher>();
            _cacheMock = new Mock<ICacheService>();
            _service = new DocumentService.Application.Services.DocumentService(_repoMock.Object, _rabbitMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnDocuments_FromRepo()
        {
            _cacheMock.Setup(c => c.GetAsync<IEnumerable<DocumentDto>>("documents_all")).ReturnsAsync((IEnumerable<DocumentDto>)null);

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Document>
            {
                new Document { DocumentId = 1, FileName = "test.pdf" }
            });

            var result = await _service.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Document)null);

            var result = await _service.GetByIdAsync(5);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldPublishEvent_AndCache()
        {
            var dto = new CreateDocumentDto
            {
                CustomerId = 1,
                FileName = "new.pdf",
                FileType = "pdf"
            };

            var entity = new Document
            {
                DocumentId = 10,
                FileName = "new.pdf",
                CustomerId = 1
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Document>())).ReturnsAsync(entity);

            var result = await _service.CreateAsync(dto);

            _rabbitMock.Verify(r => r.PublishAsync(It.IsAny<object>()), Times.Once);
            _cacheMock.Verify(c => c.SetAsync($"document_{entity.DocumentId}", It.IsAny<DocumentDto>(), It.IsAny<TimeSpan>()), Times.Once);
            Assert.Equal(10, result.DocumentId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenRepoFails()
        {
            _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _service.DeleteAsync(99);

            Assert.False(result);
        }
    }
}
