namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Document : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Documents", new[] { "User_Id" });
            DropColumn("dbo.Documents", "UserId");
            RenameColumn(table: "dbo.Documents", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Documents", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Documents", "UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Documents", new[] { "UserId" });
            AlterColumn("dbo.Documents", "UserId", c => c.Int());
            RenameColumn(table: "dbo.Documents", name: "UserId", newName: "User_Id");
            AddColumn("dbo.Documents", "UserId", c => c.Int());
            CreateIndex("dbo.Documents", "User_Id");
        }
    }
}
