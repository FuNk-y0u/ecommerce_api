using ecommerce.Models.DbModel;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class dbcontext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderModel> Orders{ get; set; }
        public dbcontext(DbContextOptions<dbcontext> options)
      : base(options)
        {
        }
    }
}
