using ASM_C6.API.DTOs.User;

namespace ASM_C6.API.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync(string? role = null);
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<(bool Success, string Message, UserDto? User)> CreateUserAsync(CreateUserDto dto);
        Task<(bool Success, string Message)> UpdateUserAsync(string id, UpdateUserDto dto);
        Task<(bool Success, string Message)> DeactivateUserAsync(string id, string currentUserId);
        Task<(bool Success, string Message)> ActivateUserAsync(string id);
        Task<(bool Success, string Message)> AssignRoleAsync(string userId, string role);
        Task<(bool Success, string Message)> RemoveRoleAsync(string userId, string role);
        Task<List<string>> GetUserRolesAsync(string userId);
    }
}