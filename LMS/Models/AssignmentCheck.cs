using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class AssignmentCheck
    {
        public Activity Activity { get; set; }
        public bool Done { get; set; }
        public bool IsLeft { get; set; }
        public bool Delayed { get; set; }
    }
}