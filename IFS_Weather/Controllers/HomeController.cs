using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using IFS_Weather.Models;
using Newtonsoft.Json.Linq;

namespace IFS_Weather.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private static readonly HttpClient client = new HttpClient();
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var model = await GetWeatherInfo();
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Login()
        {   
            /////tasarım
            ///authentication
            return View();
        }
        [NonAction]
        public async Task<List<WeatherModel>> GetWeatherInfo()
        {
            var apiKey = "ddaa7ce35cb5426c9b2111123202407";
            var url = $"http://api.worldweatheronline.com/premium/v1/weather.ashx?key={apiKey}&q=istanbul&format=json&num_of_days=7&lang=tr";
            var responseString = await client.GetStringAsync(url);
            var responseJson = JObject.Parse(responseString);
            List<WeatherModel> wlist = new List<WeatherModel>();
            var weatherList = responseJson.SelectToken("data").SelectToken("weather");
            foreach (var w in weatherList)
            {
                wlist.Add(new WeatherModel()
                {
                    WeatherDate = DateTime.Parse(w.SelectToken("date").ToString()),
                    CityName = responseJson.SelectToken("data").SelectToken("request")[0].SelectToken("query").ToString().Split(',')[0],
                    Temperature = (int)w.SelectToken("avgtempC"),
                    MainStatus = w.SelectToken("hourly")[0].SelectToken("lang_tr")[0].SelectToken("value").ToString(),
                    IconPath=w.SelectToken("hourly")[0].SelectToken("weatherIconUrl")[0].SelectToken("value").ToString()
            });
            }
            return wlist;
        }
    }
}