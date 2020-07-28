namespace IFS_Weather.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLogModels",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        LogTime = c.DateTime(nullable: false),
                        IPAddress = c.String(nullable: false),
                        Log = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.UserModels",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        UserType = c.String(nullable: false),
                        DefaultCityName = c.String(nullable: false),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.WeatherModels",
                c => new
                    {
                        WeatherId = c.Int(nullable: false, identity: true),
                        WeatherDate = c.DateTime(nullable: false),
                        CityName = c.String(nullable: false),
                        Temperature = c.Int(nullable: false),
                        MainStatus = c.String(nullable: false),
                        IconPath = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.WeatherId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeatherModels");
            DropTable("dbo.UserModels");
            DropTable("dbo.UserLogModels");
        }
    }
}
