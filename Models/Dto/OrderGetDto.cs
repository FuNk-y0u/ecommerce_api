using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ecommerce.Models.Dto
{
    public class OrderGetDto
    {
        public int orderId { get; set; }
        public string from { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }


    }
}
