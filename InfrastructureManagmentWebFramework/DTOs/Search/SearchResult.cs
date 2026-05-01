using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentWebFramework.DTOs.Search
{
    public class SearchResult
    {
        public IEnumerable<object> Users { get; set; } = Enumerable.Empty<object>();
        public IEnumerable<object> Devices { get; set; } = Enumerable.Empty<object>();
        public IEnumerable<object> Dhcp { get; set; } = Enumerable.Empty<object>();
    }

}
