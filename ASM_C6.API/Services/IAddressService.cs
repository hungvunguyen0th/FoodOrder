using ASM_C6.API.DTOs.Address;

namespace ASM_C6.API.Services
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetUserAddressesAsync(string userId);
        Task<AddressDto?> GetAddressByIdAsync(int id, string userId);
        Task<AddressDto> CreateAddressAsync(string userId, AddressDto dto);
        Task<bool> UpdateAddressAsync(int id, string userId, AddressDto dto);
        Task<bool> DeleteAddressAsync(int id, string userId);
        Task<bool> SetDefaultAddressAsync(int id, string userId);
    }
}