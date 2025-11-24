using ASM_C6.API.DTOs.Contact;

namespace ASM_C6.API.Services
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetAllContactsAsync(bool? isRead = null);
        Task<ContactDto?> GetContactByIdAsync(int id);
        Task<ContactDto> CreateContactAsync(ContactDto dto);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> RespondContactAsync(int id, string response);
        Task<bool> DeleteContactAsync(int id);
    }
}