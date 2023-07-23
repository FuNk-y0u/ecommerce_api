using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ecommerce.Models.DbModel
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public UserModel From { get; set; }
        public UserModel To { get; set; }
        public ProductModel ProductModel { get; set; }
        public int Qnty { get; set; }
    }
}
