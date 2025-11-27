using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Domain.Entities
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [MaxLength(100)]
        public string DocumentType { get; set; }   // Passport, SalarySlip

        [Required, MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(100)]
        public string FileType { get; set; }

        public long FileSize { get; set; }

        [MaxLength(1000)]
        public string StoragePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsValidated { get; set; } = false;
    }
}
