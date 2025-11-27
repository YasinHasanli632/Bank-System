using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Application.DTOs
{
    public class ProfilePhotoDto
    {
        public int PhotoId { get; set; }
        public Guid CustomerId { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long SizeBytes { get; set; }
        public string? StoragePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
