
using DocumentService.Domain.Entities;
using DocumentService.Infrastructure.Data;
using DocumentService.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure.Repositories.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DocumentDbContext _context;

        public DocumentRepository(DocumentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents.AsNoTracking().ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<Document> AddAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null)
                return false;

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Document>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Documents
                .Where(d => d.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
