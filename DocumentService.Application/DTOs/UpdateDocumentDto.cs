using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.DTOs
{
    public class UpdateDocumentDto
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string StoragePath { get; set; }
        public bool IsValidated { get; set; }
    }
}
