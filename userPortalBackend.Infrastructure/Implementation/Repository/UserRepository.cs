using Microsoft.Data.SqlClient;
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
            var result = await _dataContext.UserPortals
            .Include(user => user.AddressPortals)
            .Where(user => user.Role=="user")
            .ToListAsync();

            return result;

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
            try
            {
                var user = await _dataContext.UserPortals.AsNoTracking().FirstOrDefaultAsync(a => email == a.Email);
                user.Password = Password;
                _dataContext.Entry(user).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
                // Remove related reset password entries
                var resetPasswords = _dataContext.ResetPasswords
                    .Where(u => u.UserId == user.UserId);

                _dataContext.ResetPasswords.RemoveRange(resetPasswords);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new Exception("Failed to update the password");
            }
        }

        public async Task<UserDTO> getAdminDetail(int UserId)
        {
            var user=  await _dataContext.UserPortals.FirstOrDefaultAsync(u => u.UserId == UserId);

            return new UserDTO
            {
                FirstName= user.FirstName, 
                LastName= user.LastName, 
                Email= user.Email,
                Dob= user.Dob,
                DateOfJoining= user.DateOfJoining,
                Phone   = user.Phone,
                ImageUrl= user.ImageUrl,
                Active= user.Active,
                Gender= user.Gender
            };
        }

        public async Task updateUser(UserRegisterDTO user)
        {
            var userIdParam = new SqlParameter("@UserId", user.UserId);
            var firstNameParam = new SqlParameter("@FirstName", user.FirstName);
            var middleNameParam = new SqlParameter("@MiddleName", user.MiddleName ?? (object)DBNull.Value);
            var lastNameParam = new SqlParameter("@LastName", user.LastName);
            var genderParam = new SqlParameter("@Gender", user.Gender ?? (object)DBNull.Value);
            var dateOfJoiningParam = new SqlParameter("@DateOfJoining", user.DateOfJoining ?? (object)DBNull.Value);
            var dobParam = new SqlParameter("@Dob", user.DOB ?? (object)DBNull.Value);
            var emailParam = new SqlParameter("@Email", user.Email);
            var phoneParam = new SqlParameter("@Phone", user.Phone);
            var alternatePhoneParam = new SqlParameter("@AlternatePhone", user.AlternatePhone ?? (object)DBNull.Value);
            var imageUrlParam = new SqlParameter("@ImageUrl", user.ImageUrl ?? (object)DBNull.Value);
            var roleParam = new SqlParameter("@Role", user.Role ?? (object)DBNull.Value);
            var activeParam = new SqlParameter("@Active", user.Active );
            var passwordParam = new SqlParameter("@Password", user.Password ?? (object)DBNull.Value);
            var updatedAtParam = new SqlParameter("@UpdatedAt", DateTime.Now);

            var addressIdParam = new SqlParameter("@AddressId", user.AddressId);
            var addressLine1Param = new SqlParameter("@AddressLine1", user.AddressLine1 ?? (object)DBNull.Value);
            var cityParam = new SqlParameter("@City", user.City ?? (object)DBNull.Value);
            var stateParam = new SqlParameter("@State", user.State ?? (object)DBNull.Value);
            var countryParam = new SqlParameter("@Country", user.Country ?? (object)DBNull.Value);
            var zipCodeParam = new SqlParameter("@ZipCode", user.ZipCode ?? (object)DBNull.Value);
            var addressLine2Param = new SqlParameter("@AddressLine2", user.AddressLine2 ?? (object)DBNull.Value);
            var city2Param = new SqlParameter("@City2", user.City2 ?? (object)DBNull.Value);
            var state2Param = new SqlParameter("@State2", user.State2 ?? (object)DBNull.Value);
            var country2Param = new SqlParameter("@Country2", user.Country2 ?? (object)DBNull.Value);
            var zipCode2Param = new SqlParameter("@ZipCode2", user.ZipCode2 ?? (object)DBNull.Value);

        await _dataContext.Database.ExecuteSqlRawAsync(
        "EXEC UpdateUserAndAddress @UserId, @FirstName, @MiddleName, @LastName, @Gender, @DateOfJoining, @Dob, @Email, @Phone, @AlternatePhone, @ImageUrl, @Role, @Active, @Password, @UpdatedAt, @AddressId, @AddressLine1, @City, @State, @Country, @ZipCode, @AddressLine2, @City2, @State2, @Country2, @ZipCode2",
        userIdParam, firstNameParam, middleNameParam, lastNameParam, genderParam, dateOfJoiningParam, dobParam, emailParam, phoneParam, alternatePhoneParam, imageUrlParam, roleParam, activeParam, passwordParam, updatedAtParam,
        addressIdParam, addressLine1Param, cityParam, stateParam, countryParam, zipCodeParam, addressLine2Param, city2Param, state2Param, country2Param, zipCode2Param);
        

    }


        public async Task deleteById(int id)
        {
            try
            {
                var user = await _dataContext.UserPortals.FirstOrDefaultAsync(u=>u.UserId==id);
                var address = await _dataContext.AddressPortals.FirstOrDefaultAsync(u => u.UserId == id);
                 _dataContext.AddressPortals.RemoveRange(address);
                 _dataContext.UserPortals.RemoveRange(user);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task updateActiveStatus(int userId)
        {
            try
            {
                var user = await _dataContext.UserPortals.FirstOrDefaultAsync(u=> u.UserId==userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.Active = !user.Active;
                _dataContext.Entry(user).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
