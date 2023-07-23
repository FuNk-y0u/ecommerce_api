using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models.DbModel
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal PriceAmount { get; set; }
        public string Size { get; set; }
        public string Warrenty { get; set; }
        public string Brand { get; set; }
        public DateTime DateAdded { get; set; }
        public UserModel UserModel { get; set; }
        public CategoryModel CategoryModel { get; set; }

    }
}
