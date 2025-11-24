using ASM_C6.API.DTOs.User;
using ASM_C6.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
   public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager; _roleManager = roleManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(string? role = null)
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                if (!string.IsNullOrEmpty(role) && !roles.Contains(role)) continue;
                result.Add(new UserDto
                {
                    Id = u.Id, UserName = u.UserName, Email = u.Email, FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber, Avatar = u.Avatar, CreatedDate = u.CreatedDate, IsActive = u.IsActive, Roles = roles.ToList()
                });
            }
            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return null;
            var roles = await _userManager.GetRolesAsync(u);
            return new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate,
                Roles = roles.ToList()
            };
        }

        public async Task<(bool Success, string Message, UserDto? User)> CreateUserAsync(CreateUserDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null) return (false, "Email đã được sử dụng", null);

            var u = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            var result = await _userManager.CreateAsync(u, dto.Password);
            if (!result.Succeeded) return (false, string.Join(", ", result.Errors.Select(er => er.Description)), null);

            if (!string.IsNullOrEmpty(dto.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists) return (false, "Role không tồn tại", null);
                await _userManager.AddToRoleAsync(u, dto.Role);
            }
            var dtoRes = await GetUserByIdAsync(u.Id);
            return (true, "Tạo thành công", dtoRes);
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return (false, "Người dùng không tồn tại");
            u.FullName = dto.FullName;
            u.PhoneNumber = dto.PhoneNumber;
            u.Avatar = dto.Avatar;
            var result = await _userManager.UpdateAsync(u);
            if (!result.Succeeded) return (false, string.Join(", ", result.Errors.Select(er => er.Description)));
            return (true, "Cập nhật thành công");
        }

        public async Task<(bool Success, string Message)> DeactivateUserAsync(string id, string currentUserId)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return (false, "Người dùng không tồn tại");
            if (id == currentUserId) return (false, "Không thể tự vô hiệu hóa chính mình");
            var roles = await _userManager.GetRolesAsync(u);
            if (roles.Contains("SuperAdmin")) return (false, "Không thể vô hiệu hóa SuperAdmin");
            u.IsActive = false;
            await _userManager.UpdateAsync(u);
            return (true, "Đã vô hiệu hóa người dùng");
        }

        public async Task<(bool Success, string Message)> ActivateUserAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return (false, "Người dùng không tồn tại");
            u.IsActive = true;
            await _userManager.UpdateAsync(u);
            return (true, "Đã kích hoạt người dùng");
        }

        public async Task<(bool Success, string Message)> AssignRoleAsync(string userId, string role)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return (false, "Người dùng không tồn tại");
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists) return (false, "Role không tồn tại");
            var inRole = await _userManager.IsInRoleAsync(u, role);
            if (inRole) return (false, "Người dùng đã có role này");
            await _userManager.AddToRoleAsync(u, role);
            return (true, $"Đã gán role {role} thành công");
        }

        public async Task<(bool Success, string Message)> RemoveRoleAsync(string userId, string role)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return (false, "Người dùng không tồn tại");
            if (role == "SuperAdmin") return (false, "Không được xóa role SuperAdmin");
            var inRole = await _userManager.IsInRoleAsync(u, role);
            if (!inRole) return (false, "Người dùng không có role này");
            await _userManager.RemoveFromRoleAsync(u, role);
            return (true, $"Đã xóa role {role}");
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return new List<string>();
            var roles = await _userManager.GetRolesAsync(u);
            return roles.ToList();
        }
    }

}
