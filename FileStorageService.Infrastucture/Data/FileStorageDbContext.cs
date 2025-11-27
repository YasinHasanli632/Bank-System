using FileStorageService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Infrastructure.Data
{
    public class FileStorageDbContext:DbContext
    {
        public FileStorageDbContext(DbContextOptions<FileStorageDbContext> options):base(options)
        {
            
        }

        public DbSet<FileStorage> FileStorages { get; set; }
    }
}
