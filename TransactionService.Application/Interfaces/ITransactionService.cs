using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs;

namespace TransactionService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionReadDto>> GetAllAsync();
        Task<TransactionReadDto?> GetByIdAsync(int id);
        Task<TransactionReadDto> AddAsync(TransactionCreateDto dto);
        Task<TransactionReadDto?> UpdateAsync(int id, TransactionUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TransactionReadDto>> GetByAccountIdAsync(int accountId);
    }
}
