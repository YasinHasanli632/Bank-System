
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Data;
using TransactionService.Infrastructure.Repositories.Interfaces;

namespace TransactionService.Infrastructure.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionDbContext _context;
        private readonly DbSet<Transaction> _table;

        public TransactionRepository(TransactionDbContext context)
        {
            _context = context;
            _table = context.Set<Transaction>(); 
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _table.AsNoTracking().ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _table.FindAsync(id);
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            _table.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            _table.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _table.FindAsync(id);
            if (transaction == null)
                return false;

            
            _table.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId)
        {
            return await _table
                .Where(t => t.AccountId == accountId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
