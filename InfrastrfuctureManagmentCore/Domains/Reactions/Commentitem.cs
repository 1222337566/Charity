using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaintainanceSystem.Models
{
    public class Commentitem
    {
        public int commentid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }
        public int? todoid { get; set; }
    }
}