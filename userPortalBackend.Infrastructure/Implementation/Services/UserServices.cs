﻿using System;
using System.Collections.Generic;
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
            return await _userRepository.getAllUser();
        }

        public async Task<UserPortal> addUser(UserRegisterDTO userRegister)
        {
            if(await _userRepository.userExist(userRegister.Email)){
                throw new Exception("User is already exist");
            }

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
                Password = userRegister.Password, // Note: hash the password
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
            return await _userRepository.getUserByEmail(email);
        }
        
    }
}