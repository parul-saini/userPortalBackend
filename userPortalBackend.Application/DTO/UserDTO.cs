using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Application.DTO
{
    public class UserDTO
    {
            public int UserId { get; set; }
            public string FirstName { get; set; } = null!;
            public string? MiddleName { get; set; }
            public string LastName { get; set; } = null!;
            public string? Gender { get; set; }
            public DateOnly? DateOfJoining { get; set; }
            public DateOnly? Dob { get; set; }
            public string Email { get; set; } = null!;
            public string Phone { get; set; } = null!;
            public string? AlternatePhone { get; set; }
            public string? ImageUrl { get; set; }
            public string? Role { get; set; }
            public int? CreatedBy { get; set; }
            public DateTime? CreatedAt { get; set; }
            public bool? Active { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();
        

    }
}
