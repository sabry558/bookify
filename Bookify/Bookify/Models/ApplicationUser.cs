using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class ApplicationUser:IdentityUser
    {

        [Required, MaxLength(100)]
        public string FullName { get; set; }
        
        [MaxLength(100)]
        public string? Address { get; set; }

        [Required , MaxLength(20)]
        public string NationalId {  get; set; }

        [Required , MaxLength(50)]
        public string Nationality{get; set; }

        [Required , DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
