using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Application.DTO
{
    public class EmailDTO
    {
        public string to {  get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public EmailDTO(string to , string subject, string body) { 
            this.to = to;   
            this.subject= subject;
            this.body = body;
        }
    }
}
