namespace Takenet.Library.Logging.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Schema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TbLogMessage",
                c => new
                    {
                        LogMessageId = c.Long(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        Title = c.String(maxLength: 100),
                        Message = c.String(maxLength: 500),
                        UserName = c.String(maxLength: 50),
                        Severity = c.Int(nullable: false),
                        ApplicationName = c.String(maxLength: 50),
                        ProcessName = c.String(maxLength: 50),
                        MachineName = c.String(maxLength: 50),
                        ProcessId = c.Int(nullable: false),
                        ThreadId = c.Int(nullable: false),
                        Categories = c.String(maxLength: 100),
                        CorrelationId = c.Long(nullable: false),
                        ExtendedProperties = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.LogMessageId);
            
            CreateTable(
                "dbo.TbApplicationConfiguration",
                c => new
                    {
                        ApplicationConfigurationId = c.Guid(nullable: false),
                        ApplicationName = c.String(maxLength: 50),
                        LogRepositoryName = c.String(maxLength: 50),
                        SeverityLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ApplicationConfigurationId);
            
            CreateTable(
                "dbo.TbSeverityFilter",
                c => new
                    {
                        SeverityFilterId = c.Guid(nullable: false),
                        CategoryName = c.String(maxLength: 50),
                        MachineName = c.String(maxLength: 50),
                        MessageTitle = c.String(maxLength: 100),
                        ApplicationConfigurationId = c.Guid(nullable: false),
                        SeverityLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SeverityFilterId)
                .ForeignKey("dbo.TbApplicationConfiguration", t => t.ApplicationConfigurationId)
                .Index(t => t.ApplicationConfigurationId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TbSeverityFilter", new[] { "ApplicationConfigurationId" });
            DropForeignKey("dbo.TbSeverityFilter", "ApplicationConfigurationId", "dbo.TbApplicationConfiguration");
            DropTable("dbo.TbSeverityFilter");
            DropTable("dbo.TbApplicationConfiguration");
            DropTable("dbo.TbLogMessage");
        }
    }
}
