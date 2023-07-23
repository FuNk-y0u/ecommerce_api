using System.Security.Cryptography;
using System.Text;
using System;
using ecommerce.Repository;
using ecommerce.Interfaces;
using ecommerce.Models.DbModel;

namespace ecommerce.Services
{
    public class AccountService : IAccountService
    {
        public UserModel CreateAccount(dbcontext _context, string Email, string Username, string Password, RoleModel Role, bool isAdmin)
        {
            var Hmac = new HMACSHA512();
            var usr = new UserModel
            {
                Email = Email,
                Username = Username,
                PasswordHash = Hmac.ComputeHash(Encoding.UTF8.GetBytes(Password)),
                PasswordSalt = Hmac.Key,
                DateJoined = DateTime.Now,
                RoleModel = Role,
                EmailVerified = isAdmin
            };

            _context.Users.Add(usr);
            _context.SaveChanges();
            return usr;
        }
    }
}
