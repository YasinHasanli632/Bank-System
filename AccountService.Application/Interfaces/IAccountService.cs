using AccountService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<GetAccountDto>> GetAllAsync();
        Task<GetAccountDto?> GetByIdAsync(int id);
        Task<GetAccountDto> AddAsync(CreateAccountDto dto);
        Task<GetAccountDto?> UpdateAsync(int id, UpdateAccountDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<GetAccountDto>> GetByCustomerIdAsync(int customerId);
    }
}
