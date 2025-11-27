using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.TransactionService
{
    public record TransactionUpdatedEvent
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } 
    }
}
