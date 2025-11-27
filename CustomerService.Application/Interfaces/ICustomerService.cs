using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<bool> CustomerExistsAsync(string email);
        Task<Customer> CreateAsync(CreateCustomerDto dto);
        Task UpdateAsync(UpdateCustomerDto dto);
        Task DeleteAsync(int id);
    }
}
