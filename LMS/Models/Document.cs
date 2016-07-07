using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Uppladdningsdatum")]
        public DateTime UploadTime { get; set; }

        [Display(Name = "Ändrad av")]
        public string ModifyUserId { get; set; }

        [Display(Name = "Ändrat uppladdningsdatum")]
        public DateTime? ModifyUploadTime { get; set; }
        
        public string FileName { get; set; }
        public string FileFolder { get; set; }
        public string FileExtention { get; set; }
        
        public string UserId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }

        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
        public virtual Module Module { get; set; }
        public virtual Activity Activity { get; set; }
    }
}