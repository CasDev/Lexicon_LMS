namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_User_Course : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUsers", name: "Course_Id", newName: "CoursesId");
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_Course_Id", newName: "IX_CoursesId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_CoursesId", newName: "IX_Course_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "CoursesId", newName: "Course_Id");
        }
    }
}
