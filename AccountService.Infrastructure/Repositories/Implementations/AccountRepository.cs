
using AccountService.Domain.Entities;
using AccountService.Infrastructure.Data;
using AccountService.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _context;
        private readonly DbSet<Account> _table;

        public AccountRepository(AccountDbContext context)
        {
            _context = context;
            _table = _context.Accounts; 
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _table
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _table
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AccountId == id);
        }

        public async Task<Account> AddAsync(Account entity)
        {
            await _table.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Account> UpdateAsync(Account entity)
        {
            _table.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _table.FirstOrDefaultAsync(x => x.AccountId == id);
            if (entity is null)
                return false;

            _table.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId)
        {
            return await _table
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

    }
}
