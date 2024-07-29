using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Application.DTO
{
    public class AddressDTO
    {
            public int AddressId { get; set; }
            public int? UserId { get; set; }
            public string? AddressLine1 { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Country { get; set; }
            public string? ZipCode { get; set; }
            public string? AddressLine2 { get; set; }
            public string? City2 { get; set; }
            public string? State2 { get; set; }
            public string? Country2 { get; set; }
           public string? ZipCode2 { get; set; }
         

    }
}
