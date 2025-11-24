using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Combo;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class ComboService : IComboService
    {
        private readonly ApplicationDbContext _context;

        public ComboService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ComboDto>> GetAllCombosAsync(bool? isFeatured = null)
        {
            var query = _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.FoodItem)
                .AsQueryable();

            if (isFeatured.HasValue)
                query = query.Where(c => c.IsFeatured == isFeatured.Value);

            var combos = await query
                .OrderByDescending(c => c.IsFeatured)
                .ThenByDescending(c => c.PurchaseCount)
                .ToListAsync();

            return combos.Select(c => MapToComboDto(c)).ToList();
        }

        public async Task<ComboDto?> GetComboByIdAsync(int id)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.FoodItem)
                .FirstOrDefaultAsync(c => c.Id == id);

            return combo == null ? null : MapToComboDto(combo);
        }

        public async Task<ComboDto> CreateComboAsync(CreateComboDto dto)
        {
            var combo = new Combo
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                DiscountPercent = dto.DiscountPercent,
                IsActive = true,
                IsFeatured = false,
                CreatedDate = DateTime.Now
            };

            foreach (var item in dto.Items)
            {
                combo.ComboItems.Add(new ComboItem
                {
                    FoodItemId = item.FoodItemId,
                    Quantity = item.Quantity
                });
            }

            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

            return await GetComboByIdAsync(combo.Id) ?? new ComboDto();
        }

        public async Task<bool> UpdateComboAsync(int id, CreateComboDto dto)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
                return false;

            combo.Name = dto.Name;
            combo.Description = dto.Description;
            combo.Price = dto.Price;
            combo.ImageUrl = dto.ImageUrl;
            combo.DiscountPercent = dto.DiscountPercent;
            combo.UpdatedDate = DateTime.Now;

            // Update combo items
            _context.ComboItems.RemoveRange(combo.ComboItems);

            foreach (var item in dto.Items)
            {
                combo.ComboItems.Add(new ComboItem
                {
                    ComboId = id,
                    FoodItemId = item.FoodItemId,
                    Quantity = item.Quantity
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteComboAsync(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
                return false;

            _context.Combos.Remove(combo);
            await _context.SaveChangesAsync();
            return true;
        }

        private ComboDto MapToComboDto(Combo combo)
        {
            var originalPrice = combo.ComboItems.Sum(ci => ci.FoodItem.BasePrice * ci.Quantity);
            var savedAmount = originalPrice - combo.Price;

            return new ComboDto
            {
                Id = combo.Id,
                Name = combo.Name,
                Description = combo.Description,
                Price = combo.Price,
                ImageUrl = combo.ImageUrl,
                IsActive = combo.IsActive,
                IsFeatured = combo.IsFeatured,
                PurchaseCount = combo.PurchaseCount,
                DiscountPercent = combo.DiscountPercent,
                Items = combo.ComboItems.Select(ci => new ComboItemDto
                {
                    Id = ci.Id,
                    FoodItemId = ci.FoodItemId,
                    FoodItemName = ci.FoodItem.Name,
                    FoodItemImage = ci.FoodItem.ImageUrl,
                    FoodItemPrice = ci.FoodItem.BasePrice,
                    Quantity = ci.Quantity
                }).ToList(),
                OriginalPrice = originalPrice,
                SavedAmount = savedAmount
            };
        }
    }
}
