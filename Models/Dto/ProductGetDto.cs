using System;

namespace ecommerce.Models.Dto
{
    public class ProductGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PriceAmmount { get; set; }
        public string Size { get; set; }
        public string Warrenty { get; set; }
        public string Brand { get; set; }
        public DateTime DateAdded { get; set; }
        public int UsrId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

    }
}
