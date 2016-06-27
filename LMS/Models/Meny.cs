using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class MenyItem
    {
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public class MenyItems
    {
        public List<MenyItem> Items { get; set; }

        public MenyItems()
        {
            this.Items = new List<MenyItem>();
        }
    }
}