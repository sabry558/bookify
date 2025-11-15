namespace Bookify.DTOs.Auth
{
    public class AddRolesDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
