using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Application.DTO
{
    public class JwtCredentialDto
    {
        public string Email {  get; set; }
        public string Password { get; set; }
        public int userId { get; set; }

    }
}
