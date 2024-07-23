﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;
using userPortalBackend.presentation.Data.Models;

namespace userPortalBackend.Application.IRepository
{
    public interface IUserRepository
    {
        public Task<List<UserPortal>> getAllUser();

        public Task<UserPortal> addUser(UserPortal user);

        public Task<bool> userExist(string email);
        public Task<UserPortal> getUserByEmail(string email);
    }
}