using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore
{
    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        /// 
        [Key]
        public long? Id { get; set; }

        public string? Name { get; set; }

        public void GetProperty(string v)
        {
            throw new NotImplementedException();
        }
    }
}
