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

        public static Document SaveDocument(string folder, string fileName, HttpPostedFileBase file)
        {
            if (!File.Exists(folder +"/"+ fileName))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var path = Path.Combine(folder, fileName);
                file.SaveAs(path);

                return new Document { FileExtention = System.IO.Path.GetExtension(path), FileFolder = folder, FileName = fileName };
            }
            return null;
        }
    }
}