using ASM_C6.API.DTOs.Category;
using ASM_C6.API.DTOs.Food;
using ASM_C6.API.DTOs.Topping;

namespace ASM_C6.API.Services
{
    public interface IFoodService
    {
        // Category
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);

        // Food
        Task<List<FoodItemDto>> GetAllFoodItemsAsync(int? categoryId = null, string? search = null, bool? isFeatured = null);
        Task<FoodItemDto?> GetFoodItemByIdAsync(int id);
        Task<FoodItemDto> CreateFoodItemAsync(CreateFoodItemDto dto);
        Task<bool> UpdateFoodItemAsync(int id, UpdateFoodItemDto dto);
        Task<bool> DeleteFoodItemAsync(int id);
        Task<bool> UpdateStockAsync(int id, int quantity);

        // Topping
        Task<List<ToppingDto>> GetAllToppingsAsync();
        Task<ToppingDto?> GetToppingByIdAsync(int id);
        Task<ToppingDto> CreateToppingAsync(ToppingDto dto);
        Task<bool> UpdateToppingAsync(int id, ToppingDto dto);
        Task<bool> DeleteToppingAsync(int id);
    }
}