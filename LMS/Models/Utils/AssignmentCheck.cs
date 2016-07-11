using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    /// <summary>
    /// Menad att hållas som en "övningsuppgift". Håller koll på vilken aktivitet som övningsuppgiften tillhör
    /// och om den är färdig ( done ), ej inlämnad ( isleft ) eller försenad ( delayed ), antingen inlämnad eller
    /// ej inlämnad
    /// </summary>
    public class AssignmentStatus
    {
        public User User { get; set; }
        public Activity Activity { get; set; }
        public bool Done { get; set; }
        public bool IsLeft { get; set; }
        public bool Delayed { get; set; }
    }
}