using ASM_C6.API.Data;
using ASM_C6.API.DTOs.Category;
using ASM_C6.API.DTOs.Food;
using ASM_C6.API.DTOs.Topping;
using ASM_C6.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;
        public FoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        // CATEGORY
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
            => await _context.FoodCategories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    IsActive = c.IsActive,
                    DisplayOrder = c.DisplayOrder,
                    FoodItemCount = c.FoodItems.Count(f => f.IsActive)
                }).ToListAsync();

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var c = await _context.FoodCategories.Include(x => x.FoodItems).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return null;
            return new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                DisplayOrder = c.DisplayOrder,
                FoodItemCount = c.FoodItems.Count(f => f.IsActive)
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var c = new FoodCategory
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            _context.FoodCategories.Add(c);
            await _context.SaveChangesAsync();
            return await GetCategoryByIdAsync(c.Id)!;
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var c = await _context.FoodCategories.FindAsync(id);
            if (c == null) return false;
            c.Name = dto.Name; c.Description = dto.Description;
            c.ImageUrl = dto.ImageUrl; c.IsActive = dto.IsActive; c.DisplayOrder = dto.DisplayOrder;
            c.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var c = await _context.FoodCategories.Include(x => x.FoodItems).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null || c.FoodItems.Any()) return false;
            _context.FoodCategories.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }

        // FOOD & INGREDIENT
        public async Task<List<FoodItemDto>> GetAllFoodItemsAsync(int? categoryId = null, string? search = null, bool? isFeatured = null)
        {
            var q = _context.FoodItems.Include(f => f.Category).Include(f => f.Ingredients).Include(f => f.FoodToppings).Include(f => f.Reviews).AsQueryable();
            if (categoryId.HasValue) q = q.Where(f => f.CategoryId == categoryId);
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(f => f.Name.Contains(search));
            if (isFeatured.HasValue) q = q.Where(f => f.IsFeatured == isFeatured.Value);
            return await q.OrderByDescending(f => f.IsFeatured).ThenBy(f => f.Name)
                .Select(f => new FoodItemDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.BasePrice,
                    Cost = f.Cost,
                    ImageUrl = f.ImageUrl,
                    Calories = f.Calories,
                    Stock = f.Stock,
                    IsActive = f.IsActive,
                    IsFeatured = f.IsFeatured,
                    CategoryId = f.CategoryId,
                    CategoryName = f.Category.Name,
                    Ingredients = f.Ingredients.Select(i => new IngredientDto
                    {
                        Id = i.Id,
                        Name = i.IngredientName,
                        Cost = i.Cost,
                        Unit = i.Unit,
                        Quantity = i.Quantity
                    }).ToList(),
                    ToppingIds = f.FoodToppings.Select(ft => ft.ToppingId).ToList(),
                    AverageRating = f.Reviews.Any() ? f.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = f.Reviews.Count,
                    ViewCount = f.ViewCount,
                    PurchaseCount = f.PurchaseCount
                }).ToListAsync();
        }

        public async Task<FoodItemDto?> GetFoodItemByIdAsync(int id)
        {
            var f = await _context.FoodItems
                .Include(x => x.Category).Include(x => x.Ingredients)
                .Include(x => x.FoodToppings).Include(x => x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (f == null) return null;
            f.ViewCount++;
            await _context.SaveChangesAsync();

            return new FoodItemDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.BasePrice,
                Cost = f.Cost,
                ImageUrl = f.ImageUrl,
                Calories = f.Calories,
                Stock = f.Stock,
                IsActive = f.IsActive,
                IsFeatured = f.IsFeatured,
                CategoryId = f.CategoryId,
                CategoryName = f.Category.Name,
                Ingredients = f.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.IngredientName,
                    Cost = i.Cost,
                    Unit = i.Unit,
                    Quantity = i.Quantity
                }).ToList(),
                ToppingIds = f.FoodToppings.Select(ft => ft.ToppingId).ToList(),
                AverageRating = f.Reviews.Any() ? f.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = f.Reviews.Count,
                ViewCount = f.ViewCount,
                PurchaseCount = f.PurchaseCount
            };
        }

        public async Task<FoodItemDto> CreateFoodItemAsync(CreateFoodItemDto dto)
        {
            var f = new FoodItem
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Calories = dto.Calories,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsFeatured = false
            };
            decimal totalCost = 0;
            foreach (var ing in dto.Ingredients)
            {
                var i = new FoodIngredient
                {
                    IngredientName = ing.Name,
                    Cost = ing.Cost,
                    Unit = ing.Unit,
                    Quantity = ing.Quantity
                };
                f.Ingredients.Add(i);
                totalCost += ing.Cost * ing.Quantity;
            }
            f.Cost = totalCost;
            f.BasePrice = totalCost * 1.5m;
            if (dto.ToppingIds.Any())
                foreach (var toppingId in dto.ToppingIds)
                    f.FoodToppings.Add(new FoodTopping { ToppingId = toppingId });

            _context.FoodItems.Add(f);
            await _context.SaveChangesAsync();
            return await GetFoodItemByIdAsync(f.Id)!;
        }

        public async Task<bool> UpdateFoodItemAsync(int id, UpdateFoodItemDto dto)
        {
            var f = await _context.FoodItems.Include(x => x.Ingredients).Include(x => x.FoodToppings).FirstOrDefaultAsync(x => x.Id == id);
            if (f == null) return false;
            f.Name = dto.Name;
            f.Description = dto.Description;
            f.ImageUrl = dto.ImageUrl;
            f.Calories = dto.Calories;
            f.Stock = dto.Stock;
            f.IsActive = dto.IsActive;
            f.IsFeatured = dto.IsFeatured;
            f.CategoryId = dto.CategoryId;
            f.UpdatedDate = DateTime.Now;
            _context.FoodIngredients.RemoveRange(f.Ingredients);
            decimal totalCost = 0;
            foreach (var ing in dto.Ingredients)
            {
                var i = new FoodIngredient
                {
                    FoodItemId = id,
                    IngredientName = ing.Name,
                    Cost = ing.Cost,
                    Unit = ing.Unit,
                    Quantity = ing.Quantity
                }; _context.FoodIngredients.Add(i);
                totalCost += ing.Cost * ing.Quantity;
            }
            f.Cost = totalCost;
            f.BasePrice = totalCost * 1.5m;
            _context.FoodToppings.RemoveRange(f.FoodToppings);
            foreach (var toppingId in dto.ToppingIds)
                _context.FoodToppings.Add(new FoodTopping { FoodItemId = id, ToppingId = toppingId });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFoodItemAsync(int id)
        {
            var f = await _context.FoodItems.FindAsync(id);
            if (f == null) return false;
            _context.FoodItems.Remove(f);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            var f = await _context.FoodItems.FindAsync(id);
            if (f == null) return false;
            f.Stock += quantity;
            if (f.Stock < 0) f.Stock = 0;
            await _context.SaveChangesAsync();
            return true;
        }

        // TOPPING
        public async Task<List<ToppingDto>> GetAllToppingsAsync()
            => await _context.Toppings.Select(t => new ToppingDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Price = t.Price,
                ImageUrl = t.ImageUrl,
                IsAvailable = t.IsAvailable
            }).ToListAsync();

        public async Task<ToppingDto?> GetToppingByIdAsync(int id)
        {
            var t = await _context.Toppings.FindAsync(id);
            return t == null ? null : new ToppingDto
            {
                Id = t.Id, Name = t.Name, Description = t.Description, Price = t.Price,
                ImageUrl = t.ImageUrl, IsAvailable = t.IsAvailable
            };
        }

        public async Task<ToppingDto> CreateToppingAsync(ToppingDto dto)
        {
            var t = new Topping
            {
                Name = dto.Name, Description = dto.Description,
                Price = dto.Price, ImageUrl = dto.ImageUrl, IsAvailable = true,
                CreatedDate = DateTime.Now
            };
            _context.Toppings.Add(t);
            await _context.SaveChangesAsync();
            dto.Id = t.Id;
            return dto;
        }

        public async Task<bool> UpdateToppingAsync(int id, ToppingDto dto)
        {
            var t = await _context.Toppings.FindAsync(id);
            if (t == null) return false;
            t.Name = dto.Name; t.Description = dto.Description;
            t.Price = dto.Price; t.ImageUrl = dto.ImageUrl;
            t.IsAvailable = dto.IsAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteToppingAsync(int id)
        {
            var t = await _context.Toppings.FindAsync(id);
            if (t == null) return false;
            _context.Toppings.Remove(t);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
