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
using System.Net;
using System.Data.Entity;
using System.Diagnostics;
using System.Data;
using Newtonsoft.Json;

namespace IFS_Weather.Controllers
{
    [Authorize(Roles = "Son Kullanıcı, Yönetici")]
    public class HomeController : Controller
    {
        private IFSAppContext db = new IFSAppContext();
        private static readonly HttpClient client = new HttpClient();
        public ActionResult Index()
        {
            using (var ctx = new IFSAppContext())
            {
                var vm = new IndexViewModel();
                var user = ctx.Users.Where(w => w.Username == User.Identity.Name).SingleOrDefault();   //forms authentication identity.name i yani benzersiz olan kullanıcı adını tutuyor. kullanıcı adı cookie de tutulan bilgi
                var dateTimeNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                vm.WeatherModels = ctx.WeatherInfos.Where(w => w.CityName == user.DefaultCityName && w.WeatherDate >= dateTimeNow).ToList();
                var dataPoints = new List<DataPoint>();
                foreach (var item in vm.WeatherModels)
                {
                    dataPoints.Add(new DataPoint(item.WeatherDate, item.Temperature));
                }
                JsonSerializerSettings _jsonSetting = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                vm.DataPoints = JsonConvert.SerializeObject(dataPoints, _jsonSetting);
                return View(vm);
            }
        }
        [Authorize(Roles = "Yönetici")]
        public ActionResult Admin()
        {
            return View();
        }
        public ActionResult UserManagement()
        {
            return View(db.Users.ToList());
        }
        // GET: UserModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel userModel = db.Users.Find(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }
        // GET: UserModels/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: UserModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Username,Password,Name,UserType,DefaultCityName,Status")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(userModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userModel);
        }
        // GET: UserModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserModel userModel = db.Users.Find(id);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            return View(userModel);
        }
        // POST: UserModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Username,Password,Name,UserType,DefaultCityName,Status")] UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userModel);
        }
        // GET: UserModels/Delete/5
        public ActionResult Delete(int id)
        {
            UserModel userModel = db.Users.Find(id);
            db.Users.Remove(userModel);
            db.SaveChanges();
            return RedirectToAction("UserManagement");
        }
        public new ActionResult Profile()
        {
            using (var ctx = new IFSAppContext())
            {
                var user = ctx.Users.Where(w => w.Username == User.Identity.Name).SingleOrDefault();
                var profile = new RegisterModel()
                {
                    DefaultCityName = user.DefaultCityName,
                    Name = user.Name,
                    Username = user.Username
                };
                return View(profile);
            }
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index");
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
        [HttpPost]
        public ActionResult EditProfile(RegisterModel model)
        {
            using (var ctx = new IFSAppContext())
            {
                var user = ctx.Users.Where(w => w.Username == User.Identity.Name).SingleOrDefault();
                user.DefaultCityName = model.DefaultCityName ?? user.DefaultCityName;
                user.Name = model.Name ?? user.Name;
                user.Username = model.Username ?? user.Username;
                user.Password = model.Password != null ? BCryptHelper.HashPassword(model.Password, BCryptHelper.GenerateSalt(12)) : user.Password;
                ctx.SaveChanges();
            }
            return RedirectToAction("Profile");
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
                            MainStatus = w.SelectToken("hourly")[4].SelectToken("lang_tr")[0].SelectToken("value").ToString(),
                            IconPath = w.SelectToken("hourly")[4].SelectToken("weatherIconUrl")[0].SelectToken("value").ToString()
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
        public ActionResult WeatherManagement()
        {
            return View(db.WeatherInfos.ToList());
        }
        // GET: WeatherModels/Details/5
        public ActionResult DetailsWeather(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WeatherModel weatherModel = db.WeatherInfos.Find(id);
            if (weatherModel == null)
            {
                return HttpNotFound();
            }
            return View(weatherModel);
        }
        // GET: WeatherModels/Create
        public ActionResult CreateWeather()
        {
            return View();
        }
        // POST: WeatherModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateWeather([Bind(Include = "WeatherId,WeatherDate,CityName,Temperature,MainStatus,IconPath")] WeatherModel weatherModel)
        {
            if (ModelState.IsValid)
            {
                db.WeatherInfos.Add(weatherModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(weatherModel);
        }
        // GET: WeatherModels/Edit/5
        public ActionResult EditWeather(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WeatherModel weatherModel = db.WeatherInfos.Find(id);
            if (weatherModel == null)
            {
                return HttpNotFound();
            }
            return View(weatherModel);
        }
        // POST: WeatherModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditWeather([Bind(Include = "WeatherId,WeatherDate,CityName,Temperature,MainStatus,IconPath")] WeatherModel weatherModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(weatherModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(weatherModel);
        }
        public ActionResult DeleteWeather(int id)
        {
            WeatherModel weatherModel = db.WeatherInfos.Find(id);
            db.WeatherInfos.Remove(weatherModel);
            db.SaveChanges();
            return RedirectToAction("WeatherManagement");
        }
        // POST: WeatherModels/Delete/5
        [HttpPost, ActionName("DeleteWeather")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WeatherModel weatherModel = db.WeatherInfos.Find(id);
            db.WeatherInfos.Remove(weatherModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}