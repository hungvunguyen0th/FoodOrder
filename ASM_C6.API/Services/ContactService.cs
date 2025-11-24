using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Contact;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        public ContactService(ApplicationDbContext context) { _context = context; }

        public async Task<List<ContactDto>> GetAllContactsAsync(bool? isRead = null)
        {
            var q = _context.Contacts.AsQueryable();
            if (isRead.HasValue) q = q.Where(c => c.IsRead == isRead);
            return await q.OrderByDescending(c => c.CreatedDate)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Message = c.Message,
                    CreatedDate = c.CreatedDate,
                    IsRead = c.IsRead,
                    Response = c.Response
                }).ToListAsync();
        }

        public async Task<ContactDto?> GetContactByIdAsync(int id)
        {
            var c = await _context.Contacts.FindAsync(id);
            return c == null ? null : new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Message = c.Message,
                CreatedDate = c.CreatedDate,
                IsRead = c.IsRead,
                Response = c.Response
            };
        }

        public async Task<ContactDto> CreateContactAsync(ContactDto dto)
        {
            var c = new Contact
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Message = dto.Message,
                CreatedDate = DateTime.Now,
                IsRead = false
            };
            _context.Contacts.Add(c);
            await _context.SaveChangesAsync();
            dto.Id = c.Id; dto.CreatedDate = c.CreatedDate;
            return dto;
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var c = await _context.Contacts.FindAsync(id);
            if (c == null) return false;
            c.IsRead = true;
            c.ReadDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RespondContactAsync(int id, string response)
        {
            var c = await _context.Contacts.FindAsync(id);
            if (c == null) return false;
            c.Response = response;
            c.ResponseDate = DateTime.Now;
            c.IsRead = true;
            c.ReadDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            var c = await _context.Contacts.FindAsync(id);
            if (c == null) return false;
            _context.Contacts.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
