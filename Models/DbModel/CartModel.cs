using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models.DbModel
{
    public class CartModel
    {
        [Key]
        public int Id { get; set; }
        public UserModel UserModel { get; set; }
        public ProductModel ProductModel { get; set; }
        [DefaultValue(1)]
        public int Qnty { get; set; }

    }
}
