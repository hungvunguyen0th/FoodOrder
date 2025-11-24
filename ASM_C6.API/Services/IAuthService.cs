using ASM_C6.API.DTOs.Auth;
using ASM_C6.API.Models;

namespace ASM_C6.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginDto dto);
        Task<(bool Success, string Message, UserInfoDto? User)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Message, LoginResponseDto? Response)> RefreshTokenAsync(RefreshTokenDto dto);
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<bool> RevokeTokenAsync(string userId);
    }
}