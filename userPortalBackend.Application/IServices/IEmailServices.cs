﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;
using userPortalBackend.presentation.TempModels;

namespace userPortalBackend.Application.IServices
{
    public interface IEmailServices
    {
        public void sendEmail(EmailDTO emailDTO);

        public Task<ResetPassword> setEmailToken(ResetPassword emailCredential);
    }
}
