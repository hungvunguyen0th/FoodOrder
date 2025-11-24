using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Auth;
using ASM_C6.API.Helpers;
using ASM_C6.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtHelper jwtHelper,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtHelper = jwtHelper;
            _config = config;
        }

        public async Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.IsActive)
                return (false, "Sai tên đăng nhập hoặc mật khẩu", null);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return (false, "Sai tên đăng nhập hoặc mật khẩu", null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHelper.GenerateJwtToken(user, roles);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(
                Convert.ToDouble(_config["Jwt:RefreshTokenDurationInDays"] ?? "7"));
            await _userManager.UpdateAsync(user);

            return (true, "Đăng nhập thành công", new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.Now.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"] ?? "60")),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Avatar = user.Avatar,
                    Roles = roles.ToList()
                }
            });
        }

        public async Task<(bool Success, string Message, UserInfoDto? User)> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return (false, "Mật khẩu xác nhận không khớp", null);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors, null);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return (true, "Đăng ký thành công", new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Avatar = user.Avatar,
                Roles = new() { "Customer" }
            });
        }

        public async Task<(bool Success, string Message, LoginResponseDto? Response)> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(dto.Token);
            var userId = principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return (false, "Token không hợp lệ", null);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime < DateTime.Now)
                return (false, "Refresh token không hợp lệ hoặc đã hết hạn", null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHelper.GenerateJwtToken(user, roles);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(
                Convert.ToDouble(_config["Jwt:RefreshTokenDurationInDays"] ?? "7"));
            await _userManager.UpdateAsync(user);

            return (true, "Làm mới token thành công", new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.Now.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"] ?? "60")),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Avatar = user.Avatar,
                    Roles = roles.ToList()
                }
            });
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, "Không tồn tại user");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                return (false, "Mật khẩu xác nhận không khớp");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var err = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, err);
            }
            return (true, "Đổi mật khẩu thành công");
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return true;
        }
    }
}
