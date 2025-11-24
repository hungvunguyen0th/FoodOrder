using System.ComponentModel.DataAnnotations;

namespace ASM_C6.API.DTOs.Combo
{
    public class ComboItemDto
    {
        public int Id { get; set; }
        public int FoodItemId { get; set; }
        public string? FoodItemName { get; set; }
        public string? FoodItemImage { get; set; }
        public decimal FoodItemPrice { get; set; }
        public int Quantity { get; set; }
    }
}