using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.TransactionService
{
    public record TransactionRecordedEvent
    {
        public int TransactionId { get; init; }
        public int AccountId { get; init; }
        public int? CustomerId { get; init; }
        public string TransactionType { get; init; } = null!;
        public decimal Amount { get; init; }
        public string Currency { get; init; } = null!;
        public DateTime Timestamp { get; init; }
        public string MetadataJson { get; init; } = null!;

    }
}
