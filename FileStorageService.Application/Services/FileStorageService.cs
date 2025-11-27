using FileStorageService.Application.DTOs;
using FileStorageService.Domain.Entities;
using RedisLibrary.Interfaces;
using SharedLibrary.FileStorageService;
using SharedLibrary.Messaging.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Application.Services
{
    public class FileStorageService
      : IFileStorageService
    {
        private readonly IFileStorageRepository _repository;
        private readonly IRabbitMQPublisher _publisher;
        private readonly ICacheService _cache;

        public FileStorageService(
            IFileStorageRepository repository,
            IRabbitMQPublisher publisher,
            ICacheService cache)
        {
            _repository = repository;
            _publisher = publisher;
            _cache = cache;
        }

        private string CacheKey(int id) => $"file:{id}";

        public async Task<IEnumerable<FileStorageReadDto>> GetAllAsync()
        {
            var files = await _repository.GetAllAsync();
            return files.Select(MapToDto);
        }

        public async Task<FileStorageReadDto?> GetByIdAsync(int id)
        {
            var cached = await _cache.GetAsync<FileStorageReadDto>(CacheKey(id));
            if (cached != null) return cached;

            var file = await _repository.GetByIdAsync(id);
            if (file == null) return null;

            var dto = MapToDto(file);
            await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(10));
            return dto;
        }

        public async Task AddAsync(FileStorageCreateDto dto)
        {
            var file = new FileStorage
            {
                CustomerId = dto.CustomerId,
                FileName = dto.FileName,
                FileExtension = dto.FileExtension,
                MimeType = dto.MimeType,
                FileSize = dto.FileSize,
                FileCategory = dto.FileCategory,
                StoragePath = dto.StoragePath,
                PublicUrl = dto.PublicUrl,
                UploadedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(file);
            await _cache.SetAsync(CacheKey(file.FileId), MapToDto(file));

           
            await _publisher.PublishAsync("file_created_queue", new FileStorageCreatedEvent
            {
                FileId = file.FileId,
                CustomerId = file.CustomerId,
                FileCategory = file.FileCategory,
                PublicUrl = file.PublicUrl
            });
        }

        public async Task UpdateAsync(int id, FileStorageUpdateDto dto)
        {
            var file = await _repository.GetByIdAsync(id);
            if (file == null) return;

            file.FileName = dto.FileName;
            file.FileExtension = dto.FileExtension;
            file.MimeType = dto.MimeType;
            file.FileSize = dto.FileSize;
            file.FileCategory = dto.FileCategory;
            file.StoragePath = dto.StoragePath;
            file.PublicUrl = dto.PublicUrl;
            file.IsValidated = dto.IsValidated;
            file.ValidationMessage = dto.ValidationMessage;

            await _repository.UpdateAsync(file);
            await _cache.SetAsync(CacheKey(file.FileId), MapToDto(file));

           
            await _publisher.PublishAsync("file_updated_queue", new FileStorageUpdatedEvent
            {
                FileId = file.FileId,
                CustumerId = file.CustomerId
            });
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _cache.RemoveAsync(CacheKey(id));

            await _publisher.PublishAsync("file_deleted_queue", new FileStorageDeletedEvent
            {
                FileId = id
            });
        }

        public async Task<IEnumerable<FileStorageReadDto>> GetByCustomerIdAsync(int customerId)
        {
            var files = await _repository.GetByCustomerIdAsync(customerId);
            return files.Select(MapToDto);
        }

        private static FileStorageReadDto MapToDto(FileStorage f) => new()
        {
            FileId = f.FileId,
            CustomerId = f.CustomerId,
            FileName = f.FileName,
            FileExtension = f.FileExtension,
            MimeType = f.MimeType,
            FileSize = f.FileSize,
            FileCategory = f.FileCategory,
            PublicUrl = f.PublicUrl,
            UploadedAt = f.UploadedAt,
            IsValidated = f.IsValidated
        };
    }

}