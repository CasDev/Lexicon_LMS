using LMS.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public static class DocumentCRUD
    {
        public static Document FindAssignment(User user, Activity activity, ApplicationDbContext db, HttpServerUtilityBase server)
        {
            string folder = server.MapPath("~/documents/ovning/" + activity.Id + "/" + user.Id + "/");
            string name = "Inlämning för " + user.FirstName + " " + user.LastName;
            return FindDocument(folder, name, db);
        }

        public static bool HasDocument(string folder, string name)
        {
            return File.Exists(folder + name);
        }

        public static Document FindDocument(string folder, string name, ApplicationDbContext db)
        {
            if (Directory.Exists(folder))
            {
                return db.Documents.FirstOrDefault(d => (d.Name == name &&
                    d.FileFolder == folder));
            }
            return null;
        }

        public static bool DeleteDocument(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);

                return true;
            }
            return false;
        }

        public static IEnumerable<Document> FindAllDocumentsBelongingToModule(int id, ApplicationDbContext db)
        {
            return db.Documents.Where(d => (d.ModuleId != null && d.ModuleId == id)).ToList();
        }

        public static IEnumerable<Document> FindAllDocumentsBelongingToCourse(int id, ApplicationDbContext db)
        {
            return db.Documents.Where(d => (d.CourseId != null && d.CourseId == id)).ToList();
        }

        public static IEnumerable<Document> FindAllDocumentsBelongingToActivity(int id, ApplicationDbContext db)
        {
            return db.Documents.Where(d => (d.ActivityId != null && d.ActivityId == id)).ToList();
        }

        public static Document Update(string folder, string fileName, string extention, HttpPostedFileBase file)
        {
            DeleteDocument(folder + "/" + fileName);

            return SaveDocument(folder, fileName, extention, file);
        }

        public static Document SaveDocument(string folder, string fileName, string extention, HttpPostedFileBase file)
        {
            if (!File.Exists(folder +"/"+ fileName + extention))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var path = Path.Combine(folder, fileName + extention);
                file.SaveAs(path);

                return new Document { FileExtention = extention, FileFolder = folder, FileName = fileName + extention };
            }
            return null;
        }
    }
}