namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init_Document : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Modules", "CourseId", "dbo.Courses");
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            DropIndex("dbo.Modules", new[] { "CourseId" });
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        UploadTime = c.DateTime(nullable: false),
                        ModifyUserId = c.Int(nullable: false),
                        ModifyUploadTime = c.DateTime(),
                        FileName = c.String(),
                        FileFolder = c.String(),
                        FileExtention = c.String(),
                        UserId = c.Int(),
                        CourseId = c.Int(),
                        ModuleId = c.Int(),
                        ActivityId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activities", t => t.ActivityId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CourseId)
                .Index(t => t.ModuleId)
                .Index(t => t.ActivityId)
                .Index(t => t.User_Id);
            
            AlterColumn("dbo.Activities", "ModuleId", c => c.Int());
            AlterColumn("dbo.Modules", "CourseId", c => c.Int());
            CreateIndex("dbo.Activities", "ModuleId");
            CreateIndex("dbo.Modules", "CourseId");
            AddForeignKey("dbo.Activities", "ModuleId", "dbo.Modules", "Id");
            AddForeignKey("dbo.Modules", "CourseId", "dbo.Courses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Modules", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Documents", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Documents", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Documents", "ActivityId", "dbo.Activities");
            DropIndex("dbo.Documents", new[] { "User_Id" });
            DropIndex("dbo.Documents", new[] { "ActivityId" });
            DropIndex("dbo.Documents", new[] { "ModuleId" });
            DropIndex("dbo.Documents", new[] { "CourseId" });
            DropIndex("dbo.Modules", new[] { "CourseId" });
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            AlterColumn("dbo.Modules", "CourseId", c => c.Int(nullable: false));
            AlterColumn("dbo.Activities", "ModuleId", c => c.Int(nullable: false));
            DropTable("dbo.Documents");
            CreateIndex("dbo.Modules", "CourseId");
            CreateIndex("dbo.Activities", "ModuleId");
            AddForeignKey("dbo.Modules", "CourseId", "dbo.Courses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Activities", "ModuleId", "dbo.Modules", "Id", cascadeDelete: true);
        }
    }
}
