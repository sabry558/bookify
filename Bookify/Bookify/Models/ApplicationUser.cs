using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string NationalId {  get; set; }
        [MaxLength(50)]
        public string Nationality{get; set; }
    }
}
