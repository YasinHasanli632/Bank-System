using DocumentService.API.Controllers;
using DocumentService.Application.DTOs;
using DocumentService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Tests
{
    public class DocumentControllerTests
    {
        private readonly Mock<IDocumentService> _serviceMock;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _serviceMock = new Mock<IDocumentService>();
            _controller = new DocumentController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithDocuments()
        {
            var docs = new List<DocumentDto>
            {
                new DocumentDto { DocumentId = 1, FileName = "test.pdf" }
            };

            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(docs);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<DocumentDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenDocumentExists()
        {
            var doc = new DocumentDto { DocumentId = 1, FileName = "file.pdf" };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(doc);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<DocumentDto>(ok.Value);
            Assert.Equal("file.pdf", data.FileName);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNotExists()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((DocumentDto)null);

            var result = await _controller.GetById(5);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Document with ID 5 not found.", notFound.Value);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            var dto = new CreateDocumentDto { FileName = "file.pdf", FileType = "pdf" };
            var created = new DocumentDto { DocumentId = 1, FileName = "file.pdf" };

            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = Assert.IsType<DocumentDto>(createdResult.Value);
            Assert.Equal(1, value.DocumentId);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenSuccessful()
        {
            var dto = new UpdateDocumentDto { DocumentId = 1, FileName = "updated.pdf" };
            var updated = new DocumentDto { DocumentId = 1, FileName = "updated.pdf" };

            _serviceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(updated);

            var result = await _controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<DocumentDto>(ok.Value);
            Assert.Equal("updated.pdf", value.FileName);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            var dto = new UpdateDocumentDto { DocumentId = 2 };
            var result = await _controller.Update(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Mismatched document ID.", badRequest.Value);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeleted()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var result = await _controller.Delete(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Document with ID 1 not found.", notFound.Value);
        }
    }
}
