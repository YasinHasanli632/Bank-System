using DocumentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDto>> GetAllAsync();
        Task<DocumentDto?> GetByIdAsync(int id);
        Task<DocumentDto> CreateAsync(CreateDocumentDto dto);
        Task<DocumentDto?> UpdateAsync(UpdateDocumentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<DocumentDto>> GetByCustomerIdAsync(int customerId);
    }
}
