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
    }
}
