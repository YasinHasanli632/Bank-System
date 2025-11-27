using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Application.DTOs
{
    public class TransactionReadDto
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public string TransactionType { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "AZN";
        public DateTime Timestamp { get; set; }
        public string MetadataJson { get; set; } = null!;
    }
}
