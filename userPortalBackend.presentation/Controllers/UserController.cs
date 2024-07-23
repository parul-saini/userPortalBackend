using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using userPortalBackend.Application.DTO;
using userPortalBackend.Application.IServices;
using userPortalBackend.presentation.Data.Models;
using userPortalBackend.presentation;

namespace userPortalBackend.presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices) { 
         _userServices = userServices;
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
                var user = await _userServices.addUser(userRegister);
                return Ok("user added successfully");
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
                    return BadRequest("user not exists");
                // verify the password
                var PasswordHasher = new PasswordHasher();
                var orgPassword = PasswordHasher.VerificationPassword(loginDTO.Password,user.Password);
                if (!orgPassword)
                {
               
                    return BadRequest("Inavlid credential");
                }
                return Ok("user login successfully");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
