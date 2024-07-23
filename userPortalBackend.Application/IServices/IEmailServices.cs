using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;

namespace userPortalBackend.Application.IServices
{
    public interface IEmailServices
    {
        public void sendEmail(EmailDTO emailDTO);
    }
}
