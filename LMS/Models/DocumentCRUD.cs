﻿using LMS.Models.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public static class DocumentCRUD
    {
        public static Document FindDocument(User user, Activity activity, ApplicationDbContext db, HttpServerUtilityBase server)
        {
            if (Directory.Exists(server.MapPath("~/documents/ovning/" + activity.Id + "/" + user.Id + "/"))) {
                return db.Documents.Where(d => (d.Name == "Inlämning för " + user.FirstName + " " + user.LastName &&
                    d.FileFolder == server.MapPath("~/documents/ovning/" + activity.Id + "/" + user.Id + "/"))).FirstOrDefault();
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

        public static Document Update(string folder, string fileName, HttpPostedFileBase file)
        {
            DeleteDocument(folder + "/" + fileName);

            return SaveDocument(folder, fileName, file);
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