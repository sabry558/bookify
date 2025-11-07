using System.ComponentModel.DataAnnotations;

namespace Bookify.DTOs.Auth 
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Address { get; set; } // Optional

        public string NationalId { get; set; }   // Required
        public string Nationality { get; set; }  // Required
        public DateTime BirthDate { get; set; }  // Required
    }
}