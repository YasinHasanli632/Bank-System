
using CustomerService.Application.DTOs;
using CustomerService.Application.Interfaces;
using CustomerService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedLibrary.CustomerService;
using RedisLibrary.Interfaces;
using SharedLibrary.Messaging.Implementations;



namespace CustomerService.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IRabbitMQPublisher _rabbit;
        private readonly ICacheService _cache;
        private const string CachePrefix = "Customer_";

        public CustomerService(ICustomerRepository repository, IRabbitMQPublisher rabbit, ICacheService cache)
        {
            _repository = repository;
            _rabbit = rabbit;
            _cache = cache;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var cacheKey = $"{CachePrefix}All";
            var cached = await _cache.GetAsync<IEnumerable<CustomerDto>>(cacheKey);
            if (cached != null) return cached;

            var customers = await _repository.GetAllAsync();
            var dtos = customers.Select(MapToDto);

            await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));
            return dtos;
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var cacheKey = $"{CachePrefix}{id}";
            var cached = await _cache.GetAsync<CustomerDto>(cacheKey);
            if (cached != null) return cached;

            var customer = await _repository.GetByIdAsync(id);
            if (customer == null) return null;

            var dto = MapToDto(customer);
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
            return dto;
        }

        public async Task<bool> CustomerExistsAsync(string email)
        {
            return await _repository.CustomerExistsAsync(email);
        }

        public async Task<Customer> CreateAsync(CreateCustomerDto dto)
        {
            var entity = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                ProfilePhoto = dto.ProfilePhoto != null ? new ProfilePhoto
                {
                    FileName = dto.ProfilePhoto.FileName,
                    ContentType = dto.ProfilePhoto.ContentType,
                    SizeBytes = dto.ProfilePhoto.SizeBytes,
                    StoragePath = dto.ProfilePhoto.StoragePath
                } : null
            };

            await _repository.AddAsync(entity);

            
            await _rabbit.PublishAsync("customer_created_queue", new CustomerCreatedEvent
            {
                CustomerId = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email
            });

            await _cache.RemoveAsync($"{CachePrefix}All");
            return entity;
        }

        public async Task UpdateAsync(UpdateCustomerDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null) throw new Exception("Customer not found.");

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;

            if (dto.ProfilePhoto != null)
            {
                if (existing.ProfilePhoto == null)
                    existing.ProfilePhoto = new ProfilePhoto();

                existing.ProfilePhoto.FileName = dto.ProfilePhoto.FileName;
                existing.ProfilePhoto.ContentType = dto.ProfilePhoto.ContentType;
                existing.ProfilePhoto.SizeBytes = dto.ProfilePhoto.SizeBytes;
                existing.ProfilePhoto.StoragePath = dto.ProfilePhoto.StoragePath;
            }

            await _repository.UpdateAsync(existing);

         
            await _rabbit.PublishAsync("customer_updated_queue", new CustomerUpdatedEvent
            {
                CustomerId = existing.Id,
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                Email = existing.Email
            });

            await _cache.RemoveAsync($"{CachePrefix}All");
            await _cache.RemoveAsync($"{CachePrefix}{existing.Id}");
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);

       
            await _rabbit.PublishAsync("customer_deleted_queue", new CustomerDeletedEvent
            {
                CustomerId = id
            });

            await _cache.RemoveAsync($"{CachePrefix}All");
            await _cache.RemoveAsync($"{CachePrefix}{id}");
        }

        private static CustomerDto MapToDto(Customer c)
        {
            return new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ProfilePhoto = c.ProfilePhoto != null ? new ProfilePhotoDto
                {
                    PhotoId = c.ProfilePhoto.PhotoId,
                    CustomerId = c.ProfilePhoto.CustomerId,
                    FileName = c.ProfilePhoto.FileName,
                    ContentType = c.ProfilePhoto.ContentType,
                    SizeBytes = c.ProfilePhoto.SizeBytes,
                    StoragePath = c.ProfilePhoto.StoragePath,
                    UploadedAt = c.ProfilePhoto.UploadedAt
                } : null
            };
        }
    }
    }


