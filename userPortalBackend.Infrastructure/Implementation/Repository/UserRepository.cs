using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IRepository;
using userPortalBackend.presentation.Data.Models;
using userPortalBackend.presentation.TempModels;

namespace userPortalBackend.Infrastructure.Implementation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        public UserRepository(DataContext dataContext) {
         _dataContext = dataContext;
        }

        public async Task<List<UserPortal>> getAllUser()
        {
            return await _dataContext.UserPortals.ToListAsync();
        }

        public async Task<bool> userExist(string email)
        {
            return await _dataContext.UserPortals.AnyAsync(u => u.Email == email);
        }

        public async Task<UserPortal> addUser(UserPortal user)
        {
            await _dataContext.UserPortals.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }


        public async Task<UserPortal> getUserByEmail(string email)
        {
            return  await _dataContext.UserPortals.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<ResetPassword> setEmailToken(ResetPassword emailCredential)
        {
             await _dataContext.ResetPasswords.AddAsync(emailCredential);
            await _dataContext.SaveChangesAsync();
            return emailCredential;
        }

        public async Task<(string ResetPasswordToken, DateTime? ResetPasswordExpiry)> resetPassword(string email)
        {
            var result = await _dataContext.UserPortals.Where(user => user.Email == email)
                   .Join(_dataContext.ResetPasswords,
                   user => user.UserId,
                   reset => reset.UserId,
                   (user, reset) => new { reset.ResetPasswordToken, reset.ResetPasswordExpiry })
                   .OrderByDescending(item => item.ResetPasswordExpiry) //to get the latest email time
                   .FirstOrDefaultAsync();

            if (result != null)
            {
                return (result.ResetPasswordToken, result.ResetPasswordExpiry);
            }
            else
            {
                return (null, null); // Return null if no matching record found
            }
        }


        public async Task updatePassword(string Password,string email)
        {
            var user = await _dataContext.UserPortals.AsNoTracking().FirstOrDefaultAsync(a => email == a.Email);
            user.Password = Password;
            _dataContext.Entry(user).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }

    }
}
