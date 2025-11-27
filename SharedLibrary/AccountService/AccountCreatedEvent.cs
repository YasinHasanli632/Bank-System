

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.AccountService
{
    public record  AccountCreatedEvent
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
