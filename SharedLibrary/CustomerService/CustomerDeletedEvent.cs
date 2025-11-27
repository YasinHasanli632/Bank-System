using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.CustomerService
{
    public record CustomerDeletedEvent
    {
        public int CustomerId { get; init; }

    }
}
