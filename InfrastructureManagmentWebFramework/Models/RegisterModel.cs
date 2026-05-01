using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models
{
    public class RegisterModel
    {
        [Required,StringLength(50)]
        public string  Username { get; set; }

        [Required,StringLength(256)]

        public string Password { get; set; }


    }
}
