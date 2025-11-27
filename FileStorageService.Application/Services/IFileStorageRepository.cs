using FileStorageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Application.Services
{
    public interface IFileStorageRepository
    {
        Task<IEnumerable<FileStorage>> GetAllAsync();
        Task<FileStorage?> GetByIdAsync(int id);
        Task AddAsync(FileStorage file);
        Task UpdateAsync(FileStorage file);
        Task DeleteAsync(int id);
        Task<IEnumerable<FileStorage>> GetByCustomerIdAsync(int customerId);
    }
}
