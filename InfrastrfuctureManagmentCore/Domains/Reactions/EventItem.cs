using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;

namespace InfrastructureManagmentCore.Domains.Reactions
{
    public class EventItem
    {
        public int id { get; set; }
        public string title { get; set; }

        public string description { get; set; }

        public int startday { get; set; }
        public int startmonth { get; set; }
        public int startyear { get; set; }
        public int starthour { get; set; }
        public int startmin { get; set; }
        public int endday { get; set; }
        public int endmonth { get; set; }
        public int endyear { get; set; }
        public int endhour { get; set; }
        public int endmin { get; set; }
        

        public bool AllDays { get; set; }
        public string className { get; set; }

        public string icon { get; set; }

    }


}