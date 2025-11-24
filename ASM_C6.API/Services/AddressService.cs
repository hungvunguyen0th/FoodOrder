using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Address;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;
        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AddressDto>> GetUserAddressesAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    RecipientName = a.RecipientName,
                    PhoneNumber = a.PhoneNumber,
                    AddressLine = a.AddressLine,
                    Ward = a.Ward,
                    District = a.District,
                    IsDefault = a.IsDefault
                }).ToListAsync();
        }

        public async Task<AddressDto?> GetAddressByIdAsync(int id, string userId)
        {
            var a = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == id);
            return a == null ? null : new AddressDto
            {
                Id = a.Id,
                RecipientName = a.RecipientName,
                PhoneNumber = a.PhoneNumber,
                AddressLine = a.AddressLine,
                Ward = a.Ward,
                District = a.District,
                IsDefault = a.IsDefault
            };
        }

        public async Task<AddressDto> CreateAddressAsync(string userId, AddressDto dto)
        {
            var addr = new Address
            {
                UserId = userId,
                RecipientName = dto.RecipientName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine = dto.AddressLine,
                Ward = dto.Ward,
                District = dto.District,
                IsDefault = dto.IsDefault
            };

            if (addr.IsDefault)
            {
                // Reset default flags of other addresses
                var others = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
                foreach (var o in others)
                {
                    o.IsDefault = false;
                }
            }

            _context.Addresses.Add(addr);
            await _context.SaveChangesAsync();
            dto.Id = addr.Id;
            return dto;
        }

        public async Task<bool> UpdateAddressAsync(int id, string userId, AddressDto dto)
        {
            var addr = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (addr == null) return false;
            addr.RecipientName = dto.RecipientName;
            addr.PhoneNumber = dto.PhoneNumber;
            addr.AddressLine = dto.AddressLine;
            addr.Ward = dto.Ward;
            addr.District = dto.District;
            if (dto.IsDefault && !addr.IsDefault)
            {
                var others = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
                foreach (var o in others) o.IsDefault = false;
                addr.IsDefault = true;
            }
            else if (!dto.IsDefault && addr.IsDefault)
            {
                addr.IsDefault = false;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAddressAsync(int id, string userId)
        {
            var addr = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (addr == null) return false;
            _context.Addresses.Remove(addr);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int id, string userId)
        {
            var addr = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (addr == null) return false;
            var others = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
            foreach (var o in others) o.IsDefault = false;
            addr.IsDefault = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
