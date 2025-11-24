using ASM_C6.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASM_C6.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<FoodIngredient> FoodIngredients { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<FoodTopping> FoodToppings { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<UserDiscount> UserDiscounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemTopping> OrderItemToppings { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // FoodCategory - FoodItem
            builder.Entity<FoodCategory>()
                .HasMany(c => c.FoodItems)
                .WithOne(f => f.Category)
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // FoodItem - FoodIngredient
            builder.Entity<FoodIngredient>()
                .HasOne(i => i.FoodItem)
                .WithMany(f => f.Ingredients)
                .HasForeignKey(i => i.FoodItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // FoodItem - FoodTopping (many-to-many)
            builder.Entity<FoodTopping>()
                .HasKey(ft => new { ft.FoodItemId, ft.ToppingId });

            builder.Entity<FoodTopping>()
                .HasOne(ft => ft.FoodItem)
                .WithMany(f => f.FoodToppings)
                .HasForeignKey(ft => ft.FoodItemId);

            builder.Entity<FoodTopping>()
                .HasOne(ft => ft.Topping)
                .WithMany(t => t.FoodToppings)
                .HasForeignKey(ft => ft.ToppingId);

            // Combo - ComboItem (many-to-many-via-table)
            builder.Entity<ComboItem>()
                .HasOne(ci => ci.Combo)
                .WithMany(c => c.ComboItems)
                .HasForeignKey(ci => ci.ComboId);

            builder.Entity<ComboItem>()
                .HasOne(ci => ci.FoodItem)
                .WithMany(f => f.ComboItems)
                .HasForeignKey(ci => ci.FoodItemId);

            // Order - OrderItem
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem - Topping (many toppings per order item)
            builder.Entity<OrderItemTopping>()
                .HasOne(t => t.OrderItem)
                .WithMany(oi => oi.Toppings)
                .HasForeignKey(t => t.OrderItemId);

            builder.Entity<OrderItemTopping>()
                .HasOne(t => t.Topping)
                .WithMany()
                .HasForeignKey(t => t.ToppingId);

            // Discount - UserDiscount
            builder.Entity<UserDiscount>()
                .HasOne(ud => ud.Discount)
                .WithMany(d => d.UserDiscounts)
                .HasForeignKey(ud => ud.DiscountId);

            builder.Entity<UserDiscount>()
                .HasOne(ud => ud.User)
                .WithMany(u => u.UserDiscounts)
                .HasForeignKey(ud => ud.UserId);

            // Review - FoodItem
            builder.Entity<Review>()
                .HasOne(r => r.FoodItem)
                .WithMany(f => f.Reviews)
                .HasForeignKey(r => r.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
