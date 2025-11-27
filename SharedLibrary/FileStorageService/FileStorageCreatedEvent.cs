using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.FileStorageService
{
    public record FileStorageCreatedEvent
    {
        public int FileId { get; init; }
        public int CustomerId { get; init; }
        public string FileName { get; init; } = null!;
        public string FileCategory { get; init; } = null!;
        public string PublicUrl { get; init; } = null!;
    }
}
