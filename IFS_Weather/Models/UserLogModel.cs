using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IFS_Weather.Models
{
    [Table("USER_LOG_TAB")]
    public class UserLogModel
    {
        [Key]
        public int LogId { get; set; }
        [Required]
        public string Username { get; set; }
        public DateTime LogTime { get; set; }
        [Required]
        public string IPAddress { get; set; }
        [Required]
        public string Log { get; set; }
    }
}