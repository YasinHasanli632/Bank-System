using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Application.DTOs
{
    public class FileStorageCreateDto
    {
        public int CustomerId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public string FileCategory { get; set; }
        public string StoragePath { get; set; }
        public string? PublicUrl { get; set; }
    }
}
