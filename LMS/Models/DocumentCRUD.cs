using LMS.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public static class DocumentCRUD
    {
        public static IEnumerable<Document> FindAllDocumentsBelongingToModule(int id, ApplicationDbContext db)
        {
            return db.Documents.Where(d => d.ModuleId == id).ToList();
        }

        public static IEnumerable<Document> FindAllDocumentsBelongingToCourse(int id, ApplicationDbContext db)
        {
            return db.Documents.Where(d => d.CourseId == id).ToList();
        }

        public static IEnumerable<Document> FindAllDocumentsBelongingToActivity(int id, ApplicationDbContext db)
        {
            return db.Documents.Where(d => d.ActivityId == id).ToList();
        }

        public static bool SaveDocument(string folder, string fileName, HttpPostedFileBase file)
        {
            // TODO: need to be worked on
            return false;
        }
    }
}