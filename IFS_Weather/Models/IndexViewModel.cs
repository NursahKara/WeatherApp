using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFS_Weather.Models
{
    public class IndexViewModel
    {
        public List<WeatherModel> WeatherModels { get; set; }
        public string DataPoints { get; set; }
    }
}