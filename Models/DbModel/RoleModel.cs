using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models.DbModel
{
    public class RoleModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
