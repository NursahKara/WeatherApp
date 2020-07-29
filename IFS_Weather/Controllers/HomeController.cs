using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using IFS_Weather.Models;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using DevOne.Security.Cryptography.BCrypt;

namespace IFS_Weather.Controllers
{
    [Authorize(Roles = "Son Kullanıcı, Yönetici")]
    public class HomeController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<ActionResult> Index()
        {
            using (var ctx = new IFSAppContext())
            {
                var user = ctx.Users.Where(w => w.Username == User.Identity.Name).SingleOrDefault();   //forms authentication identity.name i yani benzersiz olan kullanıcı adını tutuyor. kullanıcı adı cookie de tutulan bilgi    
                return View(ctx.WeatherInfos.Where(w => w.CityName == user.DefaultCityName && w.WeatherDate.Date.Day >= DateTime.Now.Date.Day).ToList());  //defaultcityname in verilerini getir ve geçmiş veriler gelmesin
            }
        }
        [Authorize(Roles = "Yönetici")]
        public ActionResult Admin()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (model == null)
            {
                ViewBag.LoginError = "Hatalı giriş!";
                return View();
            }
            else
            {
                using (var ctx = new IFSAppContext())
                {
                    var u = ctx.Users.Where(w => w.Username == model.Username).SingleOrDefault();
                    if (u != null && BCryptHelper.CheckPassword(model.Password, u.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        ctx.UserLogs.Add(new UserLogModel
                        {
                            LogTime = DateTime.Now,
                            Username = u.Username,
                            IPAddress = Request.UserHostAddress,
                            Log = "Kullanıcı giriş yaptı",
                        });
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    ViewBag.LoginError = "Kullanıcı adı ya da şifre yanlış!";
                    return View();
                }
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (model == null)
            {
                ViewBag.LoginError = "Hatalı kullanıcı adı ya da şifre!";
                return RedirectToAction("Login");
            }
            else
            {
                using (var ctx = new IFSAppContext())
                {
                    var u = ctx.Users.Where(w => w.Username == model.Username).SingleOrDefault();
                    if (u != null)
                    {
                        ViewBag.RegisterError = "Kullanıcı adı mevcut";
                        return RedirectToAction("Login");
                    }
                    UserModel user = new UserModel()
                    {
                        DefaultCityName = model.DefaultCityName,
                        Name = model.Name,
                        Username = model.Username,
                        Password = BCryptHelper.HashPassword(model.Password, BCryptHelper.GenerateSalt(12)),
                        UserType = "Son Kullanıcı",
                        Status = 1,
                    };
                    ctx.Users.Add(user);
                    ctx.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [Authorize(Roles = "Yönetici")]
        public async Task<ActionResult> UpdateWeatherInfo()
        {
            var apiKey = "ddaa7ce35cb5426c9b2111123202407";
            List<string> cities = new List<string>();
            cities.Add("istanbul");
            cities.Add("izmir");
            cities.Add("ankara");
            cities.Add("sakarya");
            using (var ctx = new IFSAppContext())
            {
                var user = ctx.Users.Where(w => w.Username == User.Identity.Name).SingleOrDefault();
                foreach (var city in cities)
                {
                    var url = $"http://api.worldweatheronline.com/premium/v1/weather.ashx?key={apiKey}&q={city}&format=json&num_of_days=7&lang=tr";
                    var responseString = await client.GetStringAsync(url);
                    var responseJson = JObject.Parse(responseString);
                    List<WeatherModel> wlist = new List<WeatherModel>();
                    var weatherList = responseJson.SelectToken("data").SelectToken("weather");
                    foreach (var w in weatherList)
                    {
                        var wm = new WeatherModel()
                        {
                            WeatherDate = DateTime.Parse(w.SelectToken("date").ToString()),
                            CityName = responseJson.SelectToken("data").SelectToken("request")[0].SelectToken("query").ToString().Split(',')[0],
                            Temperature = (int)w.SelectToken("avgtempC"),
                            MainStatus = w.SelectToken("hourly")[0].SelectToken("lang_tr")[0].SelectToken("value").ToString(),
                            IconPath = w.SelectToken("hourly")[0].SelectToken("weatherIconUrl")[0].SelectToken("value").ToString()
                        };
                        bool isExists = ctx.WeatherInfos.Where(x => x.WeatherDate == wm.WeatherDate && x.CityName == wm.CityName).FirstOrDefault() != null;
                        if (!isExists)
                        {
                            wlist.Add(wm);
                        }
                    }
                    ctx.WeatherInfos.AddRange(wlist);
                }
                ctx.UserLogs.Add(new UserLogModel()
                {
                    LogTime = DateTime.Now,
                    Username = user.Username,
                    IPAddress = Request.UserHostAddress,
                    Log = "yönetici veri çekti"
                });
                ctx.SaveChanges();
            }
            return RedirectToAction("Admin");
        }
    }
}