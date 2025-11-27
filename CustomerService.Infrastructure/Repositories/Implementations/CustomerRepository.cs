using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Data;
using CustomerService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Infrastructure.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;
        private readonly DbSet<Customer> _table;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
            _table = _context.Set<Customer>();
        }

      
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _table
                .AsNoTracking()
                .ToListAsync();
        }

       
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _table
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

      
        public async Task AddAsync(Customer customer)
        {
            await _table.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        
        public async Task UpdateAsync(Customer customer)
        {
            _table.Update(customer);
            await _context.SaveChangesAsync();
        }

    
        public async Task DeleteAsync(int id)
        {
            var entity = await _table.FindAsync(id);
            if (entity != null)
            {
                _table.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

       

        
        public async Task<bool> CustomerExistsAsync(string email)
        {
            return await _table.AnyAsync(c => c.Email == email);
        }
    }
}
