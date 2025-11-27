

using FileStorageService.Domain.Entities;
using FileStorageService.Infrastructure.Data;
using FileStorageService.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Repositories.Implementations
{
    public class FileStorageRepository : IFileStorageRepository
    {
        private readonly FileStorageDbContext _context;
        private readonly DbSet<FileStorage> _table;

        public FileStorageRepository(FileStorageDbContext context)
        {
            _context = context;
            _table = context.FileStorages;  // << DbSet burada initialize edilir
        }

        public async Task<IEnumerable<FileStorage>> GetAllAsync()
        {
            return await _table
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FileStorage?> GetByIdAsync(int id)
        {
            return await _table
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FileId == id);
        }

        public async Task AddAsync(FileStorage file)
        {
            await _table.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FileStorage file)
        {
            _table.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var file = await _table.FindAsync(id);
            if (file != null)
            {
                _table.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FileStorage>> GetByCustomerIdAsync(int customerId)
        {
            return await _table
                .Where(f => f.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}