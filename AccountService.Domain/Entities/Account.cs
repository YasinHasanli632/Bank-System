using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Entities
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; } 
        [Required]
        public int CustomerId { get; set; }   

        [Required, MaxLength(50)]
        public string AccountNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "AZN";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
