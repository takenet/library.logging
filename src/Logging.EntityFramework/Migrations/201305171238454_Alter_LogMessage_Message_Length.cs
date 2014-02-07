namespace Takenet.Library.Logging.EntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alter_LogMessage_Message_Length : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TbLogMessage", "Message", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TbLogMessage", "Message", c => c.String(maxLength: 500));
        }
    }
}
