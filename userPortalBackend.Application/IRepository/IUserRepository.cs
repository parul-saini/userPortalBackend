using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;
using userPortalBackend.presentation.Data.Models;
using userPortalBackend.presentation.TempModels;

namespace userPortalBackend.Application.IRepository
{
    public interface IUserRepository
    {
        public Task<List<UserPortal>> getAllUser();

        public Task<UserPortal> addUser(UserPortal user);

        public Task<bool> userExist(string email);
        public Task<UserPortal> getUserByEmail(string email);

        public Task<ResetPassword> setEmailToken(ResetPassword emailCredential);

        public Task<(string ResetPasswordToken, DateTime? ResetPasswordExpiry)> resetPassword(string email);

        public Task updatePassword(string Password, string email);

        public Task<UserDTO> getAdminDetail(int UserId);

        public Task updateUser(UserRegisterDTO user);

        public Task deleteById(int id);

        public Task updateActiveStatus(int userId);
    }
}
