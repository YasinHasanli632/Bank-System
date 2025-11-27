using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DocumentService
{
    public record DocumentUploadedEvent
    {
        public int DocumentId { get; init; }
        public int CustomerId { get; init; }
        public string DocumentType { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public string FilePath { get; init; } = null!;
        public DateTime UploadedAt { get; init; }
    }
}
