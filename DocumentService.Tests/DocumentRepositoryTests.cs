using DocumentService.Domain.Entities;
using DocumentService.Infrastructure.Data;
using DocumentService.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Tests
{
    public class DocumentRepositoryTests
    {
        private readonly DocumentDbContext _context;
        private readonly DocumentRepository _repository;

        public DocumentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DocumentDbContext>()
                .UseInMemoryDatabase(databaseName: "DocumentTestDb")
                .EnableSensitiveDataLogging() // xəta detallı görünsün
                .Options;

            _context = new DocumentDbContext(options);
            _repository = new DocumentRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDocument()
        {
            // Arrange
            var doc = new Document
            {
                CustomerId = 1,
                DocumentType = "Passport",       // ✅ əlavə edildi
                FileName = "file1.pdf",
                FileType = "pdf",
                FileSize = 1024,
                StoragePath = "/path"
            };

            // Act
            await _repository.AddAsync(doc);
            var result = await _repository.GetByIdAsync(doc.DocumentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("file1.pdf", result.FileName);
            Assert.Equal("Passport", result.DocumentType);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDocument()
        {
            // Arrange
            var doc = new Document
            {
                CustomerId = 2,
                DocumentType = "ID Card",       // ✅ əlavə edildi
                FileName = "delete.pdf",
                FileType = "pdf",
                FileSize = 512,
                StoragePath = "/delete"
            };

            await _repository.AddAsync(doc);

            // Act
            var deleted = await _repository.DeleteAsync(doc.DocumentId);
            var check = await _repository.GetByIdAsync(doc.DocumentId);

            // Assert
            Assert.True(deleted);
            Assert.Null(check);
        }
    }
}
