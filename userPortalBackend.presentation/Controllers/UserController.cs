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
               var PasswordHasher= new PasswordHasher();
               var HashedPassword = PasswordHasher.HashedPassword(userRegister.Password);
               userRegister.Password = HashedPassword;
               //var encryptEmail= EncryptionHelper.Encryption(userRegister.Email);
               //userRegister.Email= encryptEmail;
                var user = await _userServices.addUser(userRegister);
                return Ok(new{
                    StatusCode = 200,
                    Message= "user added successfully",
                });
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/loginUser")]
        public async Task<IActionResult> loginUser(UserLoginDTO loginDTO)
        {
            try
            {
                var user = await _userServices.getUserByEmail(loginDTO.Email);

                if (user == null)
                    return BadRequest("User does not exist");

                string encryptedEmail = user.Email;
                // Decrypt the stored email from the database
                var decryptedStoredEmail = EncryptionHelper.Decryption(encryptedEmail);

                // Compare the decrypted email with the one provided by the user
                if (decryptedStoredEmail != loginDTO.Email)
                {
                    return BadRequest("Invalid credentials");
                }


                // verify the password
                var PasswordHasher = new PasswordHasher();
                var orgPassword = PasswordHasher.VerificationPassword(loginDTO.Password,user.Password);
                if (!orgPassword)
                {
               
                    return BadRequest("Inavlid credential");
                }

                var jwtCredential = new JwtCredentialDto{
                     userId= user.UserId,
                     Email= user.Email,
                     Password= loginDTO.Password
                };

                var jwtToken = JwtTokenGenerator.GenerateToken(jwtCredential);
                return Ok(new {
                    token= jwtToken,
                    Message= "user login successfully"
                });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
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
                        Message = "User Not Found"
                    }
                );

                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var emailToken = Convert.ToBase64String(tokenBytes);
                var resetPassword = new ResetPassword
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
                _emailServices.setEmailToken(resetPassword);
                return Ok(new
                {
                    statusCode = 200,
                    Message = "Sent Email Successfully!"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
