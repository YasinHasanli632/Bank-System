using RedisLibrary.Interfaces;
using SharedLibrary.Messaging.Implementations;
using SharedLibrary.TransactionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Repositories.Interfaces;


namespace TransactionService.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly IRabbitMQPublisher _publisher;
        private readonly ICacheService _cache;

        public TransactionService(
            ITransactionRepository repository,
            IRabbitMQPublisher publisher,
            ICacheService cache)
        {
            _repository = repository;
            _publisher = publisher;
            _cache = cache;
        }

        private string CacheKey(int id) => $"transaction:{id}";

        private static TransactionReadDto MapToDto(Transaction t) => new()
        {
            TransactionId = t.TransactionId,
            AccountId = t.AccountId,
            CustomerId = t.CustomerId,
            TransactionType = t.TransactionType,
            Amount = t.Amount,
            Currency = t.Currency,
            Timestamp = t.Timestamp,
            MetadataJson = t.MetadataJson
        };


        public async Task<IEnumerable<TransactionReadDto>> GetAllAsync()
        {
            var transactions = await _repository.GetAllAsync();
            return transactions.Select(MapToDto);
        }


        public async Task<TransactionReadDto?> GetByIdAsync(int id)
        {
            var cached = await _cache.GetAsync<TransactionReadDto>(CacheKey(id));
            if (cached != null) return cached;

            var transaction = await _repository.GetByIdAsync(id);
            if (transaction == null) return null;

            var dto = MapToDto(transaction);
            await _cache.SetAsync(CacheKey(id), dto, TimeSpan.FromMinutes(10));

            return dto;
        }


        public async Task<TransactionReadDto> AddAsync(TransactionCreateDto dto)
        {
            var entity = new Transaction
            {
                AccountId = dto.AccountId,
                CustomerId = dto.CustomerId,
                TransactionType = dto.TransactionType,
                Amount = dto.Amount,
                Currency = dto.Currency,
                MetadataJson = dto.MetadataJson,
                Timestamp = DateTime.UtcNow
            };

            var added = await _repository.AddAsync(entity);

            await _cache.SetAsync(CacheKey(added.TransactionId), MapToDto(added));

            await _publisher.PublishAsync("transaction.recorded", new TransactionRecordedEvent
            {
                TransactionId = added.TransactionId,
                AccountId = added.AccountId,
                CustomerId = added.CustomerId,
                TransactionType = added.TransactionType,
                Amount = added.Amount,
                Currency = added.Currency,
                Timestamp = added.Timestamp
            });

            return MapToDto(added);
        }


        public async Task<TransactionReadDto?> UpdateAsync(int id, TransactionUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.TransactionType = dto.TransactionType;
            existing.Amount = dto.Amount;
            existing.Currency = dto.Currency;
            existing.MetadataJson = dto.MetadataJson;

            var updated = await _repository.UpdateAsync(existing);

            await _cache.SetAsync(CacheKey(updated.TransactionId), MapToDto(updated));

            await _publisher.PublishAsync("transaction.updated", new TransactionUpdatedEvent
            {
                TransactionId = updated.TransactionId,
                AccountId = updated.AccountId,
                CustomerId = updated.CustomerId,
                TransactionType = updated.TransactionType,
                Amount = updated.Amount,

            });

            return MapToDto(updated);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return false;

            await _cache.RemoveAsync(CacheKey(id));

            await _publisher.PublishAsync("transaction.deleted", new TransactionDeletedEvent
            {
                TransactionId = id
            });

            return true;
        }


        public async Task<IEnumerable<TransactionReadDto>> GetByAccountIdAsync(int accountId)
        {
            var transactions = await _repository.GetByAccountIdAsync(accountId);
            return transactions.Select(MapToDto);
        }


    }
}
