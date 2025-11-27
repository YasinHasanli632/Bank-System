using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int AccountId { get; set; }

        public int CustomerId { get; set; }   

        [Required, MaxLength(50)]
        public string TransactionType { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "AZN";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(4000)]
        public string MetadataJson { get; set; }
    }
}
