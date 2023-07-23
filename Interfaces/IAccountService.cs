using ecommerce.Models.DbModel;
using ecommerce.Repository;

namespace ecommerce.Interfaces
{
    public interface IAccountService
    {
        public UserModel CreateAccount(dbcontext _context, string Email, string Username, string Password, RoleModel Role, bool isAdmin);

    }
}
