using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IServices;
using userPortalBackend.presentation.Data.Models;
using userPortalBackend.presentation;
using System.Security.Cryptography;
using userPortalBackend.presentation.TempModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace userPortalBackend.presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailServices;
        public UserController(IUserServices userServices, IConfiguration configuration,IEmailServices emailServices) { 
         _userServices = userServices;
         _configuration = configuration;
           _emailServices = emailServices;
        }

        [HttpGet]
        public async Task<List<UserPortal>> GetAllUser() { 
            return await _userServices.getAllUser();
        }

        [HttpPost]
        [Route("/RegisterUser")]
        public async Task<IActionResult> registerUser(UserRegisterDTO userRegister)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                var userIdClaimId=  int.Parse(userIdClaim);
                //i made a email obj before hashing credentials
                var _emailModel = new EmailDTO
               (
                   userRegister.Email,
                   "Your Credentials",
                   EmailBody.EmailStringbodyForSendingCredentails(userRegister)
               );
                var PasswordHasher= new PasswordHasher();
               var HashedPassword = PasswordHasher.HashedPassword(userRegister.Password);
               userRegister.Password = HashedPassword;
               var user = await _userServices.addUser(userRegister, userIdClaimId);
                _emailServices.sendEmail(_emailModel);
                return Ok(new{
                    StatusCode = 200,
                    Message= "User added successfully ,Also sent credential to user email",
                });
            }
            catch (Exception ex) { 
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = "Failed to add user",
                    Exception= ex.Message,
                });
            }
        }

        [HttpPost]
        [Route("/updateUser")]
        public async Task<IActionResult> UpdateUser(UserRegisterDTO user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("user is null");
                }
                //var PasswordHasher = new PasswordHasher();
                //var HashedPassword = PasswordHasher.HashedPassword(user?.Password);
                //user.Password = HashedPassword;
                await _userServices.updateUser(user);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "User updated successfully"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message,
                   // Error = ex
                });
            }
        }

        [HttpPost]
        [Route("/loginUser")]
        public async Task<IActionResult> loginUser(UserLoginDTO loginDTO)
        {
            try
            {
                var user = await _userServices.loginUser(loginDTO);

                if (user == null)
                    return BadRequest(new
                    {
                        StatusCode = 401,
                        Message = "Inavlid credential!"
                    });

                // verify the password
                var PasswordHasher = new PasswordHasher();
                var orgPassword = PasswordHasher.VerificationPassword(loginDTO.Password,user.Password);
                if (!orgPassword)
                {
               
                    return BadRequest(new{ StatusCode = 401,
                    Message = "Invalid credential!"});
                }

                var jwtCredential = new JwtCredentialDto{
                     userId= user.UserId,
                     Email= user.Email,
                     Password= loginDTO.Password
                };

                var jwtToken = JwtTokenGenerator.GenerateToken(jwtCredential);
                return Ok(new {
                    token= jwtToken,
                    Message= "User login successfully"
                });
            }
            catch (Exception ex) {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = "Failed To Login!",
                });
            }
        }

        [HttpGet]
        [Route("/reset-password-email/{email}")]
        public async Task<IActionResult> sendEmail(string email)
        {
            try
            {
                var user = await _userServices.getUserByEmail(email);
                if (user == null) return NotFound(
                    new
                    {
                        statusCode = 404,
                        message = "This Not A Registered Email"
                    }
                );

                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var emailToken = Convert.ToBase64String(tokenBytes);
                var resetPasswordObj = new ResetPassword
                {
                    UserId = user.UserId,
                    ResetPasswordToken = emailToken,
                    ResetPasswordExpiry = DateTime.Now.AddMinutes(20)
                };

                var from = _configuration["Emailsettings:From"];
                var _emailModel = new EmailDTO
                (
                    email,
                    "Reset Password",
                    EmailBody.EmailStringbody(email, emailToken)
                );

                _emailServices.sendEmail( _emailModel );
                _emailServices.setEmailToken(resetPasswordObj);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Sent Email Successfully!"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = "Failed to Send Email",
                });
            }

        }


        [HttpPost]
        [Route("/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO PasswordToReset)
        {
            try
            {
                var PasswordHasher = new PasswordHasher();
                var HashedPassword = PasswordHasher.HashedPassword(PasswordToReset.Password);
                (string resetPasswordToken, DateTime? resetPasswordExpiry)= await _userServices.resetPassword(PasswordToReset.email);
              //  Console.WriteLine(resetPasswordToken);
              //  Console.WriteLine("code ", PasswordToReset);
                if (resetPasswordToken != null && PasswordToReset.code != null &&
    !           resetPasswordToken.ToString().Equals(PasswordToReset.code.ToString()))
                {
                    return BadRequest("Invalid Token,Sent Email Again");
                }

                if (resetPasswordExpiry.HasValue && resetPasswordExpiry < DateTime.Now)
                {
                    return BadRequest("Token has expired,,Sent Email Again");
                }

                await _userServices.updatePassword(HashedPassword, PasswordToReset.email);
                return Ok(new
                {
                    statusCode=200,
                    Message= "Updated Password SuccessFully!"
                });
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users =  await _userServices.getAllUser();
                var usersResponse = users.Select(user => new
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    DateOfJoining = user.DateOfJoining,
                    Dob = user.Dob,
                    Email = user.Email,
                    Phone = user.Phone,
                    AlternatePhone = user.AlternatePhone,
                    ImageUrl = user.ImageUrl,
                    Active = user.Active,
                    Addresses = user.AddressPortals.Select(address => new AddressDTO
                    {
                        AddressId = address.AddressId,
                        UserId = address.UserId,
                        AddressLine1 = address.AddressLine1,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        ZipCode = address.ZipCode,
                        AddressLine2 = address.AddressLine2,
                        City2 = address.City2,
                        State2 = address.State2,
                        Country2 = address.Country2,
                        ZipCode2 = address.ZipCode2
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    StatusCode = 200,
                    Data = usersResponse,
                    Message= " Users Fetch SuccessFully",
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message,                  
                });
            }
        }


        [Authorize]
        [HttpGet]
        [Route("/getAdmindetail")]
        public async Task<IActionResult> GetAdminDetail()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                
                if (userIdClaim == null)
                {
                    return Unauthorized(
                         "Invalid Token"
                    );
                }

                int userId = int.Parse(userIdClaim);
                var user = await _userServices.getAdminDetail(userId);

                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Data = user,
                    Message = "User fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 500,
                    Data = null,
                    Message = ex.Message
                });
            }
        }


        [HttpDelete]
        [Route("/deleteById/{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                await _userServices.deleteById(id);
                return Ok(new
                {
                     StatusCode= 200,
                     Message= "Deleted successfully",
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = " Failed to delete",
                    Error= ex.Message,
                });
            }
        }

        [HttpGet]
        [Route("/updateActiveStatus/{userId}")]
        public async Task<IActionResult> updateActiveStatus(int userId)
        {
            try
            {
                await _userServices.updateActiveStatus(userId);
                return Ok(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Data = null,
                    Message = "Update Active status successfully"
                });
            }
            catch(Exception ex) {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 200,
                    Data = null,
                    Message = "Failed to update Active status"
                });

            }
        }

        [HttpGet]
        [Route("/getUserDetails/{userId}")]
        public async Task<IActionResult> getUserDetails(int userId)
        {
            try
            {
                var user= await _userServices.getUserDetail(userId);    
                return Ok(user);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
