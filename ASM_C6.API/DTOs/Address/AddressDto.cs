using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Address
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}