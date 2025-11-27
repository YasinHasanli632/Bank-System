
using DocumentService.Application.DTOs;
using DocumentService.Application.Interfaces;
using DocumentService.Domain.Entities;

using DocumentService.Infrastructure.Repositories.Interface;
using RedisLibrary.Interfaces;
using SharedLibrary.DocumentService;
using SharedLibrary.Messaging.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly IRabbitMQPublisher _rabbit;
        private readonly ICacheService _cache;

        public DocumentService(IDocumentRepository repository, IRabbitMQPublisher rabbit, ICacheService cache)
        {
            _repository = repository;
            _rabbit = rabbit;
            _cache = cache;
        }

        public async Task<IEnumerable<DocumentDto>> GetAllAsync()
        {
            const string cacheKey = "documents_all";
            var cached = await _cache.GetAsync<IEnumerable<DocumentDto>>(cacheKey);
            if (cached != null) return cached;

            var docs = await _repository.GetAllAsync();
            var dtos = docs.Select(MapToDto);

            await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));
            return dtos;
        }

        public async Task<DocumentDto?> GetByIdAsync(int id)
        {
            var cacheKey = $"document_{id}";
            var cached = await _cache.GetAsync<DocumentDto>(cacheKey);
            if (cached != null) return cached;

            var doc = await _repository.GetByIdAsync(id);
            if (doc == null) return null;

            var dto = MapToDto(doc);
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
            return dto;
        }

        public async Task<DocumentDto> CreateAsync(CreateDocumentDto dto)
        {
            var entity = new Document
            {
                CustomerId = dto.CustomerId,
                DocumentType = dto.DocumentType,
                FileName = dto.FileName,
                FileType = dto.FileType,
                FileSize = dto.FileSize,
                StoragePath = dto.StoragePath
            };

            var added = await _repository.AddAsync(entity);

            
            await _rabbit.PublishAsync("document_created_queue", new DocumentUploadedEvent
            {
                DocumentId = added.DocumentId,
                CustomerId = added.CustomerId,
                DocumentType = added.DocumentType,
                FileName = added.FileName
            });

           
            var dtoResult = MapToDto(added);
            await _cache.SetAsync($"document_{added.DocumentId}", dtoResult, TimeSpan.FromMinutes(5));

            return dtoResult;
        }

        public async Task<DocumentDto?> UpdateAsync(UpdateDocumentDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.DocumentId);
            if (existing == null) return null;

            existing.DocumentType = dto.DocumentType;
            existing.FileName = dto.FileName;
            existing.FileType = dto.FileType;
            existing.FileSize = dto.FileSize;
            existing.StoragePath = dto.StoragePath;
            existing.IsValidated = dto.IsValidated;

            var updated = await _repository.UpdateAsync(existing);

           
            await _rabbit.PublishAsync("document_updated_queue", new DocumentUpdatedEvent
            {
                DocumentId = updated.DocumentId,
                CustomerId = updated.CustomerId,
                DocumentType = updated.DocumentType,
                FileName = updated.FileName
            });

          
            var dtoResult = MapToDto(updated);
            await _cache.SetAsync($"document_{updated.DocumentId}", dtoResult, TimeSpan.FromMinutes(5));

            return dtoResult;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return false;

            
            await _rabbit.PublishAsync("document_deleted_queue", new DocumentDeletedEvent
            {
                DocumentId = id
            });

          
            await _cache.RemoveAsync($"document_{id}");
            return true;
        }

        public async Task<IEnumerable<DocumentDto>> GetByCustomerIdAsync(int customerId)
        {
            var cacheKey = $"documents_customer_{customerId}";
            var cached = await _cache.GetAsync<IEnumerable<DocumentDto>>(cacheKey);
            if (cached != null) return cached;

            var docs = await _repository.GetByCustomerIdAsync(customerId);
            var dtos = docs.Select(MapToDto);

            await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));
            return dtos;
        }

        private static DocumentDto MapToDto(Document d) => new()
        {
            DocumentId = d.DocumentId,
            CustomerId = d.CustomerId,
            DocumentType = d.DocumentType,
            FileName = d.FileName,
            FileType = d.FileType,
            FileSize = d.FileSize,
            StoragePath = d.StoragePath,
            UploadedAt = d.UploadedAt,
            IsValidated = d.IsValidated
        };
    }
}


