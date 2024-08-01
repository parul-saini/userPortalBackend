using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
            var parameters = new[]
            {
        new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId },
        new SqlParameter("@FirstName", SqlDbType.NVarChar, 50) { Value = user.FirstName },
        new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50) { Value = (object)user.MiddleName ?? DBNull.Value },
        new SqlParameter("@LastName", SqlDbType.NVarChar, 50) { Value = user.LastName },
        new SqlParameter("@Gender", SqlDbType.VarChar, 20) { Value = (object)user.Gender ?? DBNull.Value },
        new SqlParameter("@DateOfJoining", SqlDbType.Date) { Value = (object)user.DateOfJoining ?? DBNull.Value },
        new SqlParameter("@Dob", SqlDbType.Date) { Value = (object)user.DOB ?? DBNull.Value },
        new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = user.Email },
        new SqlParameter("@Phone", SqlDbType.NVarChar, 15) { Value = user.Phone },
        new SqlParameter("@AlternatePhone", SqlDbType.NVarChar, 15) { Value = (object)user.AlternatePhone ?? DBNull.Value },
        new SqlParameter("@ImageUrl", SqlDbType.NVarChar, 1000) { Value = (object)user.ImageUrl ?? DBNull.Value },
        new SqlParameter("@AddressId", SqlDbType.Int) { Value = user.AddressId },
        new SqlParameter("@AddressLine1", SqlDbType.NVarChar, 255) { Value = (object)user.AddressLine1 ?? DBNull.Value },
        new SqlParameter("@City", SqlDbType.NVarChar, 100) { Value = (object)user.City ?? DBNull.Value },
        new SqlParameter("@State", SqlDbType.NVarChar, 100) { Value = (object)user.State ?? DBNull.Value },
        new SqlParameter("@Country", SqlDbType.NVarChar, 100) { Value = (object)user.Country ?? DBNull.Value },
        new SqlParameter("@ZipCode", SqlDbType.NVarChar, 10) { Value = (object)user.ZipCode ?? DBNull.Value },
        new SqlParameter("@AddressLine2", SqlDbType.NVarChar, 255) { Value = (object)user.AddressLine2 ?? DBNull.Value },
        new SqlParameter("@City2", SqlDbType.NVarChar, 100) { Value = (object)user.City2 ?? DBNull.Value },
        new SqlParameter("@State2", SqlDbType.NVarChar, 100) { Value = (object)user.State2 ?? DBNull.Value },
        new SqlParameter("@Country2", SqlDbType.NVarChar, 100) { Value = (object)user.Country2 ?? DBNull.Value },
        new SqlParameter("@ZipCode2", SqlDbType.NVarChar, 10) { Value = (object)user.ZipCode2 ?? DBNull.Value },
    };

            await _dataContext.Database.ExecuteSqlRawAsync(
                "EXEC UpdateUserAndAddress @UserId, @FirstName, @MiddleName, @LastName, @Gender, @DateOfJoining, @Dob, @Email, @Phone, @AlternatePhone, @ImageUrl, @AddressId, @AddressLine1, @City, @State, @Country, @ZipCode, @AddressLine2, @City2, @State2, @Country2, @ZipCode2",
                parameters);
        }


        public async Task deleteById(int id)
        {
            try
            {
                var user = await _dataContext.UserPortals.FirstOrDefaultAsync(u=>u.UserId==id);
                var address = await _dataContext.AddressPortals.FirstOrDefaultAsync(u => u.UserId == id);
                if (address != null)
                {
                    _dataContext.AddressPortals.RemoveRange(address);
                }
                if (user != null)
                {
                    _dataContext.UserPortals.Remove(user);
                }
                
                await _dataContext.SaveChangesAsync();

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

        public async Task<UserDTO> getUserDetail(int UserId)
        {
            try
            {
                // Fetch the user details from the userPortal table
                var user = await _dataContext.UserPortals.FirstOrDefaultAsync(u => u.UserId == UserId);
                if (user == null)
                {
                    throw new Exception($"User with ID {UserId} was not found.");
                }
                // Fetch the addresses associated with the user from the addressPortal table
                var addresses = await _dataContext.AddressPortals
                    .Where(a => a.UserId == UserId)
                    .Select(a => new AddressDTO
                    {
                        AddressId = a.AddressId,
                        UserId = a.UserId,
                        AddressLine1 = a.AddressLine1,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        ZipCode = a.ZipCode,
                        AddressLine2 = a.AddressLine2,
                        City2 = a.City2,
                        State2 = a.State2,
                        Country2 = a.Country2,
                        ZipCode2 = a.ZipCode2
                    }).ToListAsync() ?? new List<AddressDTO>(); ;

                // Map the user and address data to the UserDTO
                return new UserDTO
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user.MiddleName,
                    Gender = user.Gender,
                    DateOfJoining = user.DateOfJoining,
                    Email = user.Email,
                    Dob = user.Dob,
                    Phone = user.Phone,
                    AlternatePhone = user.AlternatePhone,
                    ImageUrl = user.ImageUrl,
                    Role = user.Role,
                    CreatedBy = user.CreatedBy,
                    CreatedAt = user.CreatedAt,
                    Active = user.Active,
                    UpdatedAt = user.UpdatedAt,
                    Addresses = addresses
                };


            }
            catch (Exception ex) {
                throw new Exception("Error retrieving user details", ex);
            }
        }
    }
}
