using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IServices;
using userPortalBackend.presentation.Data.Models;
using userPortalBackend.presentation;
using System.Security.Cryptography;
using userPortalBackend.presentation.TempModels;

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
               var user = await _userServices.addUser(userRegister);
                _emailServices.sendEmail(_emailModel);
                return Ok(new{
                    StatusCode = 200,
                    Message= "user added successfully",
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
                    Message = "Inavlid credential!"});
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
                Console.WriteLine(resetPasswordToken);
                Console.WriteLine("code ", PasswordToReset);
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
    }
}
