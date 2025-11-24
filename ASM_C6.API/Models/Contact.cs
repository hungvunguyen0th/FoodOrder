namespace ASM_C6.API.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public string? Response { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}