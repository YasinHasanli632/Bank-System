using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.AccountService
{
    public record AccountDeletedEvent
    {
        public int AccountId { get; init; }
    }
}
