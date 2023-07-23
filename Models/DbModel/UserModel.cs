using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models.DbModel
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public DateTime DateJoined { get; set; }
        public RoleModel RoleModel { get; set; }

        [DefaultValue(false)]
        public bool EmailVerified { get; set; }

    }
}
