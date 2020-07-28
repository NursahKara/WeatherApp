using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFS_Weather.Models
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string DefaultCityName { get; set; }
    }
}