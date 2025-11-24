using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.User
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
    }
}