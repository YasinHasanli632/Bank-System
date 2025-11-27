using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.FileStorageService
{
    public record FileStorageDeletedEvent
    {
        public int FileId { get; init; }
    }
}
