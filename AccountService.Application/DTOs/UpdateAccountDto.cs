using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.DTOs
{
    public class UpdateAccountDto
    {

        [Required]
        public int AccountId { get; set; }

        [Required, MaxLength(50)]
        public string AccountNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "AZN";
    }
}
