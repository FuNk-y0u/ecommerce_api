using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models.Dto
{
    public class CartDto
    {
        public string Token { get; set; }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Qnty { get; set; }

    }
}
