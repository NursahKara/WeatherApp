using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IFS_Weather.Models
{
    public class IFSAppContext : DbContext
    {
        public IFSAppContext() : base("ifscnn")
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserLogModel> UserLogs { get; set; }
        public DbSet<WeatherModel> WeatherInfos { get; set; }
    }
}