using ecommerce.Models;
using ecommerce.Models.DbModel;
using ecommerce.Models.Dto;

namespace ecommerce.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(UserModel user);
        public int ValidateToken(string token);
        public string VerificationGenerateEmail(SignupDto user);
        public string ValidateVerifyToken(string token);
    }
}
