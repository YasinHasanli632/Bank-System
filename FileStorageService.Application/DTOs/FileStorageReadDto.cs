using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Application.DTOs
{
    public class FileStorageReadDto
    {
        public int FileId { get; set; }
        public int CustomerId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public string FileCategory { get; set; }
        public string PublicUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsValidated { get; set; }
    }
}
