using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageService.Domain.Entities
{
    public class FileStorage
    {
        [Key]
        public int FileId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        // File meta
        [Required, MaxLength(255)]
        public string FileName { get; set; } // original name

        [Required, MaxLength(50)]
        public string FileExtension { get; set; } // .jpg, .pdf etc.

        [MaxLength(100)]
        public string MimeType { get; set; } // image/jpeg, application/pdf etc.

        public long FileSize { get; set; } // bytes

        // File category
        [Required, MaxLength(50)]
        public string FileCategory { get; set; }
        // Examples: "ProfilePhoto", "LoanDocument", "SalarySlip", "Passport"

        // Storage info
        [Required, MaxLength(1000)]
        public string StoragePath { get; set; } // e.g. "/storage/customers/1/photos/photo_9238.jpg"

        [MaxLength(1000)]
        public string PublicUrl { get; set; } // internal app URL, CDN URL etc.

        // Validation flags (mainly for loan documents)
        public bool IsValidated { get; set; }
        public string? ValidationMessage { get; set; }

        // Upload information
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
