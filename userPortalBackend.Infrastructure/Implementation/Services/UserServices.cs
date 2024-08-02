using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IRepository;
using userPortalBackend.Application.IServices;
using userPortalBackend.presentation.Data.Models;

namespace userPortalBackend.Infrastructure.Implementation.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository) { 
          _userRepository = userRepository;
        }

        public async Task<List<UserPortal>> getAllUser()
        {
            var usersList =  await _userRepository.getAllUser();
            foreach (var user in usersList) { 
                var decryptedEmail= EncryptionDecryptionHandler.Decryption(user.Email);
                user.Email = decryptedEmail;
            }
            return usersList;
        }

        public async Task<UserPortal> addUser(UserRegisterDTO userRegister,int createdBy)
        {
           var isUserExist = await this.getUserByEmail(userRegister.Email);
           if (isUserExist != null){
                throw new Exception("User is already exist");
           }
            var encryptedEmail = EncryptionDecryptionHandler.Encryption(userRegister.Email);
            userRegister.Email = encryptedEmail;

            var user = new UserPortal
            {
                FirstName = userRegister.FirstName,
                MiddleName = userRegister.MiddleName,
                LastName = userRegister.LastName,
                Gender = userRegister.Gender,
                DateOfJoining = userRegister.DateOfJoining.HasValue
                ? DateOnly.FromDateTime(userRegister.DateOfJoining.Value)
                : (DateOnly?)null,
                Dob = userRegister.DOB.HasValue
                ? DateOnly.FromDateTime(userRegister.DOB.Value)
                : (DateOnly?)null,
                Email = userRegister.Email,
                Phone = userRegister.Phone,
                AlternatePhone = userRegister.AlternatePhone,
                Password = userRegister.Password, //  hashed  password
                Role = userRegister.Role,
                ImageUrl = userRegister.ImageUrl,
                CreatedBy= createdBy,
                AddressPortals = new List<AddressPortal>
                {
                    new AddressPortal
                    {
                        AddressLine1 = userRegister.AddressLine1,
                        City = userRegister.City,
                        State = userRegister.State,
                        Country = userRegister.Country,
                        ZipCode = userRegister.ZipCode,
                        AddressLine2 = userRegister.AddressLine2,
                        City2 = userRegister.City2,
                        State2 = userRegister.State2,
                        Country2 = userRegister.Country2,
                        ZipCode2 = userRegister.ZipCode2
                    }
                }
            };

            return await _userRepository.addUser(user);
        }

        public async Task<UserPortal> getUserByEmail(string email)
        {
            var encryptedEmail = EncryptionDecryptionHandler.Encryption(email);
            return await _userRepository.getUserByEmail(encryptedEmail);
        }

        public async Task<UserPortal> loginUser(UserLoginDTO logindto)
        {
            //encrypt the email at all, you need to do it with a common salt/key. Otherwise, how are you going to select a user by his email address from the db to check whether the hashed password is correct?
            var decryptedEmail = EncryptionDecryptionHandler.Encryption(logindto.Email);
            return await _userRepository.getUserByEmail(decryptedEmail);
        }

        public async Task<(string resetPasswordToken, DateTime? resetPasswordExpiry)> resetPassword(string email)
        {
            try
            {
                var decryptedEmail = EncryptionDecryptionHandler.Encryption(email);
                 return await _userRepository.resetPassword(decryptedEmail);
 
            }
            catch (Exception ex) {
                throw;
            }
        }


        public async Task updatePassword(string Password, string email)
        {
            var decryptedEmail = EncryptionDecryptionHandler.Encryption(email);
            await _userRepository.updatePassword(Password, decryptedEmail);
        }

        public async Task<UserDTO> getAdminDetail(int userId)
        {
            return await _userRepository.getAdminDetail(userId);
        }

        public async Task updateUser(UserRegisterDTO user)
        {
            var encryptedEmail = EncryptionDecryptionHandler.Encryption(user.Email);
            user.Email = encryptedEmail;
            await _userRepository.updateUser(user);
        }

        public async Task deleteById(int id )
        {
            try
            {
                 await _userRepository.deleteById(id);
            }
            catch (Exception ex) { 
               throw new Exception(ex.Message);
            }
        }

        public async Task updateActiveStatus(int userId)
        {
            try
            {
                await _userRepository.updateActiveStatus(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDTO> getUserDetail(int UserId)
        {
            var user= await _userRepository.getUserDetail(UserId);
            user.Email= EncryptionDecryptionHandler.Decryption(user.Email);
            return user
                ;
        }
    }
}
