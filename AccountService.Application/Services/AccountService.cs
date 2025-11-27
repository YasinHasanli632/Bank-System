using AccountService.Application.DTOs;
using AccountService.Application.Interfaces;
using AccountService.Domain.Entities;

using AccountService.Infrastructure.Repositories.Interface;
using Microsoft.Extensions.Caching.Memory;
using RedisLibrary.Interfaces;
using SharedLibrary.AccountService;
using SharedLibrary.Messaging.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Services
{
    public class AccountService : IAccountService
    {
       private readonly IAccountRepository _repository;
    private readonly IRabbitMQPublisher _publisher;
    private readonly ICacheService _cache;  

    public AccountService(
        IAccountRepository repository,
        IRabbitMQPublisher publisher,
        ICacheService cache)
    {
        _repository = repository;
        _publisher = publisher;
        _cache = cache;
    }

    private static GetAccountDto Map(Account acc) => new()
    {
        AccountId = acc.AccountId,
        CustomerId = acc.CustomerId,
        AccountNumber = acc.AccountNumber,
        Balance = acc.Balance,
        Currency = acc.Currency,
        CreatedAt = acc.CreatedAt
    };

    public async Task<IEnumerable<GetAccountDto>> GetAllAsync()
    {
        var cacheKey = "accounts_all";

        var cached = await _cache.GetAsync<IEnumerable<Account>>(cacheKey);

        if (cached is not null)
            return cached.Select(Map);

        var list = await _repository.GetAllAsync();

        await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(5));

        return list.Select(Map);
    }

    public async Task<GetAccountDto?> GetByIdAsync(int id)
    {
        var cacheKey = $"account_{id}";

        var cached = await _cache.GetAsync<Account>(cacheKey);
        if (cached is not null)
            return Map(cached);

        var acc = await _repository.GetByIdAsync(id);
        if (acc == null)
            return null;

        await _cache.SetAsync(cacheKey, acc, TimeSpan.FromMinutes(5));

        return Map(acc);
    }

    public async Task<GetAccountDto> AddAsync(CreateAccountDto dto)
    {
        var entity = new Account
        {
            CustomerId = dto.CustomerId,
            AccountNumber = dto.AccountNumber,
            Balance = dto.Balance,
            Currency = dto.Currency,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _repository.AddAsync(entity);

        // Cache temizlənir
        await _cache.RemoveAsync("accounts_all");

        await _publisher.PublishAsync("account_created_queue", new AccountCreatedEvent
        {
            AccountId = added.AccountId,
            CustomerId = added.CustomerId,
            AccountNumber = added.AccountNumber,
            Balance = added.Balance,
            Currency = added.Currency,
            CreatedAt = added.CreatedAt
        });

        return Map(added);
    }

    public async Task<GetAccountDto?> UpdateAsync(int id, UpdateAccountDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.AccountNumber = dto.AccountNumber;
        existing.Balance = dto.Balance;
        existing.Currency = dto.Currency;

        var updated = await _repository.UpdateAsync(existing);

        // Cache cleaning
        await _cache.RemoveAsync("accounts_all");
        await _cache.RemoveAsync($"account_{id}");

        await _publisher.PublishAsync("account_updated_queue", new AccountUpdatedEvent
        {
            AccountId = updated.AccountId,
            AccountNumber = updated.AccountNumber,
            Balance = updated.Balance,
            Currency = updated.Currency
        });

        return Map(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
            return false;

        await _cache.RemoveAsync("accounts_all");
        await _cache.RemoveAsync($"account_{id}");

        await _publisher.PublishAsync("account_deleted_queue", new AccountDeletedEvent
        {
            AccountId = id
        });

        return true;
    }

    public async Task<IEnumerable<GetAccountDto>> GetByCustomerIdAsync(int customerId)
    {
        var cacheKey = $"accounts_customer_{customerId}";

        var cached = await _cache.GetAsync<IEnumerable<Account>>(cacheKey);

        if (cached is not null)
            return cached.Select(Map);

        var list = await _repository.GetByCustomerIdAsync(customerId);

        await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(5));

        return list.Select(Map);
    }
    }
}
