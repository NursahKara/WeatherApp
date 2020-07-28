using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IFS_Weather.Models
{
    [Table("USER_TAB")]
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserType { get; set; }
        [Required]
        public string DefaultCityName { get; set; }
        public short Status { get; set; }
    }
}