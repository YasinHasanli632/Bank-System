
using AccountService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.Repositories.Interface
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<Account> AddAsync(Account account);
        Task<Account?> UpdateAsync(Account account);
        Task<bool> DeleteAsync(int id);


        Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId);
    }
}
