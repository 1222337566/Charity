using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.Models
{
    public class ParameterAggModel
    {
        public Dictionary<string, Int16> parameter1 { get; set; } 
        public Dictionary<string, int> parameter2 { get; set; }
        public Dictionary<string, Int64> parameters4 { get; set; }
        public Dictionary<string,DateTime> parameters3 { get; set; }
        public Dictionary<string, string> parameters5 { get; set; }
        public Dictionary<string,Guid> parameter6 { get; set; }
        public Dictionary<string, decimal> parameters7 { get; set; }
        public Dictionary<string,DataTable> Table { get; set; }
        public Dictionary<string, DateOnly> parameters8 { get; set; }
        public Dictionary<string, Byte> parameters9 { get; set; }
        public Dictionary<string, Boolean> parameters10 { get; set; }
    }
}
