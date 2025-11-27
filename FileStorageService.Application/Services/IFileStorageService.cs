using FileStorageService.Application.DTOs;

namespace FileStorageService.Application.Services
{
    public interface IFileStorageService
    {
        Task<IEnumerable<FileStorageReadDto>> GetAllAsync();
        Task<FileStorageReadDto?> GetByIdAsync(int id);
        Task AddAsync(FileStorageCreateDto dto);
        Task UpdateAsync(int id, FileStorageUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<FileStorageReadDto>> GetByCustomerIdAsync(int customerId);
    }
}