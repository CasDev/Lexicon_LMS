namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Document_ModifyUserId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Documents", "ModifyUserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Documents", "ModifyUserId", c => c.Int(nullable: false));
        }
    }
}
