using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Application.DTOs
{
    public class TransactionUpdateDto
    {
        public string TransactionType { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "AZN";
        public string MetadataJson { get; set; } = null!;
        
    }
}
