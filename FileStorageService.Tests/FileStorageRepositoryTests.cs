using FileStorageService.Domain.Entities;
using FileStorageService.Infrastructure.Data;
using FileStorageService.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Tests
{
    public class FileStorageRepositoryTests
    {
        private readonly FileStorageDbContext _context;
        private readonly FileStorageRepository _repository;

        public FileStorageRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FileStorageDbContext>()
                .UseInMemoryDatabase(databaseName: "FileStorageTestDb")
                .EnableSensitiveDataLogging()
                .Options;

            _context = new FileStorageDbContext(options);
            _repository = new FileStorageRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddFile()
        {
            // Arrange
            var file = new FileStorage
            {
                CustomerId = 1,
                FileName = "test.pdf",
                FileExtension = ".pdf",
                MimeType = "application/pdf",
                FileSize = 1024,
                FileCategory = "Document",
                StoragePath = "/uploads/test.pdf",
                PublicUrl = "http://localhost/test.pdf"
            };

            // Act
            await _repository.AddAsync(file);
            var result = await _repository.GetByIdAsync(file.FileId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test.pdf", result.FileName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveFile()
        {
            // Arrange
            var file = new FileStorage
            {
                CustomerId = 2,
                FileName = "delete.pdf",
                FileExtension = ".pdf",
                MimeType = "application/pdf",
                FileSize = 500,
                FileCategory = "Invoice",
                StoragePath = "/uploads/delete.pdf",
                PublicUrl = "http://localhost/delete.pdf"
            };

            await _repository.AddAsync(file);

            // Act
            await _repository.DeleteAsync(file.FileId);
            var result = await _repository.GetByIdAsync(file.FileId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFiles()
        {
            // Arrange
            _context.FileStorages.Add(new FileStorage
            {
                CustomerId = 3,
                FileName = "a.pdf",
                FileExtension = ".pdf",
                MimeType = "application/pdf",
                FileSize = 200,
                FileCategory = "Doc",
                StoragePath = "/uploads/a.pdf",
                PublicUrl = "http://localhost/a.pdf"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotEmpty(result);
        }
    }
}
