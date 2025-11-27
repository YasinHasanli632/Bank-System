using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Application.DTOs
{
    public class CustomerWithDetailsDto : CustomerDto
    {
        public string? ProfilePhotoUrl { get; set; }
        public int AccountCount { get; set; }
        public int DocumentCount { get; set; }
    }
}
