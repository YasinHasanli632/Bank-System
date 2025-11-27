using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DocumentService
{
    public record DocumentUpdatedEvent
    {
        public int CustomerId { get; set; }
        public int DocumentId { get; init; }
        public string DocumentType { get; init; } = null!;
        public string FileName { get; init; } = null!;
    }
}
