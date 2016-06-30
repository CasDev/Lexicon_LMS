﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Display(Name = "Typ")]
        public string Type { get; set; }
        
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Starttid")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Sluttid")]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Deadline")]
        public DateTime? Deadline { get; set; }

        public int? ModuleId { get; set; }

        public virtual Module Module { get; set; }
    }
}