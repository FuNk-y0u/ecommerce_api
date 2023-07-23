using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PriceAmount { get; set; }
        public string Size { get; set; }
        public string Warrenty { get; set; }
        public string Brand { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
