using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IFS_Weather.Models
{
    [Table("Weather_Info_Tab")]
    public class WeatherModel
    {
        [Key]
        public int WeatherId { get; set; }
        public DateTime WeatherDate { get; set; }
        [Required]
        public string CityName { get; set; }
        public int Temperature { get; set; }
        [Required]
        public string MainStatus { get; set; }
        [Required]
        public string IconPath { get; set; }
    }
}