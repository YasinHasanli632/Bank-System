using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DocumentService
{
    public record DocumentDeletedEvent
    {
        public int DocumentId { get; init; }
    }
}
