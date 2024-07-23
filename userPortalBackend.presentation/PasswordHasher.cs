using Microsoft.AspNetCore.Identity;

namespace userPortalBackend.presentation
{
    public class PasswordHasher
    {
        private readonly IPasswordHasher<object> _passwordHasher;

        public PasswordHasher()
        {
            _passwordHasher = new PasswordHasher<object>();  
        }

        public string HashedPassword(string password)
        {
            return _passwordHasher.HashPassword(null,password);
        }

        public bool VerificationPassword(string password, string hashedPassword) {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result== PasswordVerificationResult.Success;
        }

    }
}
