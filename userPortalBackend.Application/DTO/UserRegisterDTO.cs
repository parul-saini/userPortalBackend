using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Application.DTO
{
    public class UserRegisterDTO
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DOB { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AlternatePhone { get; set; }
        public string Password { get; set; }
        public string Role { get;set; }
        public string ImageUrl {  get; set; }

        // Address fields
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string AddressLine2 { get; set; }
        public string City2 { get; set; }
        public string State2 { get; set; }
        public string Country2 { get; set; }
        public string ZipCode2 { get; set; }
    }
}
