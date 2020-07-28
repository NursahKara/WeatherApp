namespace IFS_Weather.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameTables : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserLogModels", newName: "USER_LOG_TAB");
            RenameTable(name: "dbo.UserModels", newName: "USER_TAB");
            RenameTable(name: "dbo.WeatherModels", newName: "Weather_Info_Tab");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Weather_Info_Tab", newName: "WeatherModels");
            RenameTable(name: "dbo.USER_TAB", newName: "UserModels");
            RenameTable(name: "dbo.USER_LOG_TAB", newName: "UserLogModels");
        }
    }
}
