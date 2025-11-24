using ASM_C6.API.DTOs.Combo;

namespace ASM_C6.API.Services
{
    public interface IComboService
    {
        Task<List<ComboDto>> GetAllCombosAsync(bool? isFeatured = null);
        Task<ComboDto?> GetComboByIdAsync(int id);
        Task<ComboDto> CreateComboAsync(CreateComboDto dto);
        Task<bool> UpdateComboAsync(int id, CreateComboDto dto);
        Task<bool> DeleteComboAsync(int id);
    }
}