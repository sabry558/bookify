using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class ApplicationUser:IdentityUser
    {

        [Required, MaxLength(100)]
        public string FullName { get; set; }
        
        [Required, MaxLength(100)]
        public string Address { get; set; }

        [Required]
        public string NationalId {  get; set; }

        [MaxLength(50)]
        public string Nationality{get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
