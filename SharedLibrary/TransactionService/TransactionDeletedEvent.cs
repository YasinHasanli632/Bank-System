









using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.TransactionService
{
    public record TransactionDeletedEvent
    {
        public int TransactionId { get; init; }
    }
}
