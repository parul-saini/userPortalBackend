using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;
using userPortalBackend.presentation.Data.Models;

namespace userPortalBackend.Application.IServices
{
    public interface IUserServices
    {
        public Task<List<UserPortal>> getAllUser();

        public Task<UserPortal> addUser(UserRegisterDTO userRegister);

        public Task<UserPortal> getUserByEmail(string email);

        public Task<UserPortal> loginUser(UserLoginDTO logindto);

        public Task<(string resetPasswordToken, DateTime? resetPasswordExpiry)> resetPassword(string email);

        Task updatePassword(string Password, string email);
    }
}
