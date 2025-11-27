using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; } 

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation / conceptual only
        public virtual ProfilePhoto ProfilePhoto { get; set; }
    }
    public class ProfilePhoto
    {
        [Key]
        public int PhotoId { get; set; } 

        [Required]
        public Guid CustomerId { get; set; }

        [Required, MaxLength(255)]
        public string FileName { get; set; }

        [Required, MaxLength(100)]
        public string ContentType { get; set; }

        public long SizeBytes { get; set; }

        [MaxLength(1000)]
        public string StoragePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
