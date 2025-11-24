using ASM_C6.API.Data;
using ASM_C6.API.Helpers;
using ASM_C6.API.Models;
using ASM_C6.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ASM_C6.API.DTOs.Auth;
using ASM_C6.API.DTOs.User;
using ASM_C6.API.DTOs.Address;
using ASM_C6.API.DTOs.Order;
using ASM_C6.API.DTOs.Discount;
using ASM_C6.API.DTOs.Review;
using ASM_C6.API.DTOs.Contact;
using ASM_C6.API.DTOs.Category;
using ASM_C6.API.DTOs.Food;
using ASM_C6.API.DTOs.Topping;
using ASM_C6.API.DTOs.Combo;

var builder = WebApplication.CreateBuilder(args);

// 1. ADD DB CONTEXT & IDENTITY
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// API vue
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Port Vue frontend
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 2. JWT CONFIG
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 4. INJECT SERVICES & HELPERS
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IAddressService, AddressService>();

// 5. SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "ASM_C6_API", Version = "v1" });
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id="Bearer" }
            }, new string[] { }
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASM_C6 API V1");
    c.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// app.UseHttpsRedirection();
app.UseCors("AllowVue");
app.UseAuthentication();
app.UseAuthorization();

// ===================== ENDPOINTS (GROUPED BY TAGS) =====================

// AUTH
app.MapPost("/api/auth/register", async (RegisterDto dto, IAuthService service) =>
{
    var res = await service.RegisterAsync(dto);
    return res.Success ? Results.Ok(ApiResponse<UserInfoDto>.SuccessResponse(res.User!, res.Message))
                       : Results.BadRequest(ApiResponse<object>.ErrorResponse(res.Message));
});
app.MapPost("/api/auth/login", async (LoginDto dto, IAuthService service) =>
{
    var res = await service.LoginAsync(dto);
    return res.Success ? Results.Ok(ApiResponse<LoginResponseDto>.SuccessResponse(res.Response!, res.Message))
                       : Results.Unauthorized();
});
app.MapPost("/api/auth/refresh-token", async (RefreshTokenDto dto, IAuthService service) =>
{
    var res = await service.RefreshTokenAsync(dto);
    return res.Success ? Results.Ok(ApiResponse<LoginResponseDto>.SuccessResponse(res.Response!, res.Message))
                       : Results.Unauthorized();
});
app.MapPost("/api/auth/change-password", async (ChangePasswordDto dto, HttpContext ctx, IAuthService service) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();
    var res = await service.ChangePasswordAsync(userId, dto);
    return res.Success ? Results.Ok(ApiResponse<object>.SuccessResponse(null, res.Message))
                       : Results.BadRequest(ApiResponse<object>.ErrorResponse(res.Message));
});

// USERS & ADDRESS
app.MapGet("/api/users", async (string? role, IUserService svc) => Results.Ok(await svc.GetAllUsersAsync(role)));
app.MapGet("/api/users/{id}", async (string id, IUserService svc) =>
{
    var res = await svc.GetUserByIdAsync(id);
    return res == null ? Results.NotFound() : Results.Ok(res);
});
app.MapPost("/api/users", async (CreateUserDto dto, IUserService svc) =>
{
    var res = await svc.CreateUserAsync(dto);
    return res.Success ? Results.Created($"/api/users/{res.User!.Id}", res.User) : Results.BadRequest(res.Message);
});
app.MapPut("/api/users/{id}", async (string id, UpdateUserDto dto, IUserService svc) =>
{
    var res = await svc.UpdateUserAsync(id, dto);
    return res.Success ? Results.Ok(res.Message) : Results.BadRequest(res.Message);
});
app.MapPut("/api/users/{id}/deactivate", async (string id, HttpContext ctx, IUserService svc) =>
{
    var curUser = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
    var res = await svc.DeactivateUserAsync(id, curUser);
    return res.Success ? Results.Ok(res.Message) : Results.BadRequest(res.Message);
});
app.MapPut("/api/users/{id}/activate", async (string id, IUserService svc) =>
{
    var res = await svc.ActivateUserAsync(id);
    return res.Success ? Results.Ok(res.Message) : Results.BadRequest(res.Message);
});
app.MapGet("/api/users/{userId}/roles", async (string userId, IUserService svc) =>
    Results.Ok(await svc.GetUserRolesAsync(userId)));
app.MapPost("/api/users/{userId}/roles/{role}", async (string userId, string role, IUserService svc) =>
{
    var res = await svc.AssignRoleAsync(userId, role);
    return res.Success ? Results.Ok(res.Message) : Results.BadRequest(res.Message);
});
app.MapDelete("/api/users/{userId}/roles/{role}", async (string userId, string role, IUserService svc) =>
{
    var res = await svc.RemoveRoleAsync(userId, role);
    return res.Success ? Results.Ok(res.Message) : Results.BadRequest(res.Message);
});

// ADDRESS
app.MapGet("/api/addresses", async (HttpContext ctx, IAddressService svc) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    return string.IsNullOrEmpty(userId) ? Results.Unauthorized() : Results.Ok(await svc.GetUserAddressesAsync(userId));
});
app.MapPost("/api/addresses", async (HttpContext ctx, AddressDto dto, IAddressService svc) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();
    var res = await svc.CreateAddressAsync(userId, dto);
    return Results.Created($"/api/addresses/{res.Id}", res);
});
app.MapPut("/api/addresses/{id}", async (int id, HttpContext ctx, AddressDto dto, IAddressService svc) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    return await svc.UpdateAddressAsync(id, userId!, dto)
        ? Results.Ok() : Results.NotFound();
});
app.MapDelete("/api/addresses/{id}", async (int id, HttpContext ctx, IAddressService svc) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    return await svc.DeleteAddressAsync(id, userId!) ? Results.Ok() : Results.NotFound();
});

// CATEGORY
app.MapGet("/api/categories", async (IFoodService svc) =>
    Results.Ok(await svc.GetAllCategoriesAsync()));
app.MapGet("/api/categories/{id}", async (int id, IFoodService svc) =>
{
    var c = await svc.GetCategoryByIdAsync(id);
    return c == null ? Results.NotFound() : Results.Ok(c);
});
app.MapPost("/api/categories", async (CreateCategoryDto dto, IFoodService svc) =>
    Results.Created($"/api/categories", await svc.CreateCategoryAsync(dto)));
app.MapPut("/api/categories/{id}", async (int id, UpdateCategoryDto dto, IFoodService svc) =>
    await svc.UpdateCategoryAsync(id, dto) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/categories/{id}", async (int id, IFoodService svc) =>
    await svc.DeleteCategoryAsync(id) ? Results.Ok() : Results.NotFound());

// FOOD ITEMS
app.MapGet("/api/foods", async (int? categoryId, string? search, bool? isFeatured, IFoodService svc) =>
    Results.Ok(await svc.GetAllFoodItemsAsync(categoryId, search, isFeatured)));
app.MapGet("/api/foods/{id}", async (int id, IFoodService svc) =>
{
    var food = await svc.GetFoodItemByIdAsync(id);
    return food == null ? Results.NotFound() : Results.Ok(food);
});
app.MapPost("/api/foods", async (CreateFoodItemDto dto, IFoodService svc) =>
    Results.Created($"/api/foods", await svc.CreateFoodItemAsync(dto)));
app.MapPut("/api/foods/{id}", async (int id, UpdateFoodItemDto dto, IFoodService svc) =>
    await svc.UpdateFoodItemAsync(id, dto) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/foods/{id}", async (int id, IFoodService svc) =>
    await svc.DeleteFoodItemAsync(id) ? Results.Ok() : Results.NotFound());
app.MapPatch("/api/foods/{id}/stock", async (int id, int quantity, IFoodService svc) =>
    await svc.UpdateStockAsync(id, quantity) ? Results.Ok() : Results.NotFound());

// TOPPING
app.MapGet("/api/toppings", async (IFoodService svc) =>
    Results.Ok(await svc.GetAllToppingsAsync()));
app.MapGet("/api/toppings/{id}", async (int id, IFoodService svc) =>
{
    var t = await svc.GetToppingByIdAsync(id);
    return t == null ? Results.NotFound() : Results.Ok(t);
});
app.MapPost("/api/toppings", async (ToppingDto dto, IFoodService svc) =>
    Results.Created($"/api/toppings", await svc.CreateToppingAsync(dto)));
app.MapPut("/api/toppings/{id}", async (int id, ToppingDto dto, IFoodService svc) =>
    await svc.UpdateToppingAsync(id, dto) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/toppings/{id}", async (int id, IFoodService svc) =>
    await svc.DeleteToppingAsync(id) ? Results.Ok() : Results.NotFound());

// COMBO
app.MapGet("/api/combos", async (bool? isFeatured, IComboService svc) =>
    Results.Ok(await svc.GetAllCombosAsync(isFeatured)));
app.MapGet("/api/combos/{id}", async (int id, IComboService svc) =>
{
    var combo = await svc.GetComboByIdAsync(id);
    return combo == null ? Results.NotFound() : Results.Ok(combo);
});
app.MapPost("/api/combos", async (CreateComboDto dto, IComboService svc) =>
    Results.Created($"/api/combos", await svc.CreateComboAsync(dto)));
app.MapPut("/api/combos/{id}", async (int id, CreateComboDto dto, IComboService svc) =>
    await svc.UpdateComboAsync(id, dto) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/combos/{id}", async (int id, IComboService svc) =>
    await svc.DeleteComboAsync(id) ? Results.Ok() : Results.NotFound());


// ORDERS
app.MapPost("/api/orders", async (CreateOrderDto dto, HttpContext ctx, IOrderService svc) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    var res = await svc.CreateOrderAsync(dto, userId);
    return res.Success ? Results.Created($"/api/orders/{res.Order!.Id}", res.Order)
                       : Results.BadRequest(res.Message);
});
app.MapGet("/api/orders", async (HttpContext ctx, IOrderService svc, int? status, int pn=1, int ps=10) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    return Results.Ok(await svc.GetOrdersAsync(userId, status, pn, ps));
});
app.MapGet("/api/orders/{id}", async (int id, IOrderService svc) =>
{
    var order = await svc.GetOrderByIdAsync(id);
    return order == null ? Results.NotFound() : Results.Ok(order);
});
app.MapPut("/api/orders/{id}/status", async (int id, UpdateOrderStatusDto dto, IOrderService svc) =>
{
    return await svc.UpdateOrderStatusAsync(id, dto.Status) ? Results.Ok() : Results.NotFound();
});
app.MapPost("/api/orders/{id}/cancel", async (int id, HttpContext ctx, IOrderService svc) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    return await svc.CancelOrderAsync(id, userId) ? Results.Ok() : Results.BadRequest();
});
app.MapGet("/api/orders/statistics", async (IOrderService svc) => Results.Ok(await svc.GetOrderStatisticsAsync()));

// DISCOUNT
app.MapGet("/api/discounts", async (bool? isActive, IDiscountService service) =>
    Results.Ok(await service.GetAllDiscountsAsync(isActive)));
app.MapGet("/api/discounts/{id}", async (int id, IDiscountService service) =>
{
    var d = await service.GetDiscountByIdAsync(id);
    return d == null ? Results.NotFound() : Results.Ok(d);
});
app.MapGet("/api/discounts/code/{code}", async (string code, IDiscountService service) =>
{
    var d = await service.GetDiscountByCodeAsync(code);
    return d == null ? Results.NotFound() : Results.Ok(d);
});
app.MapPost("/api/discounts", async (DiscountDto dto, IDiscountService service) =>
    Results.Created($"/api/discounts/{dto.Code}", await service.CreateDiscountAsync(dto)));
app.MapPut("/api/discounts/{id}", async (int id, DiscountDto dto, IDiscountService service) =>
    await service.UpdateDiscountAsync(id, dto) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/discounts/{id}", async (int id, IDiscountService service) =>
    await service.DeleteDiscountAsync(id) ? Results.Ok() : Results.NotFound());
app.MapPost("/api/discounts/assign/{discountId}/{userId}", async (int discountId, string userId, IDiscountService service) =>
    await service.AssignDiscountToUserAsync(discountId, userId) ? Results.Ok() : Results.BadRequest());
app.MapGet("/api/discounts/user/{userId}", async (string userId, IDiscountService service) =>
    Results.Ok(await service.GetUserDiscountsAsync(userId)));
app.MapPost("/api/discounts/validate", async (string code, decimal orderAmount, HttpContext ctx, IDiscountService service) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    var res = await service.ValidateDiscountAsync(code, orderAmount, userId);
    return !res.IsValid ? Results.BadRequest(res.Message) : Results.Ok(res.Discount);
});

// REVIEW
app.MapGet("/api/reviews/food/{foodItemId}", async (int foodItemId, IReviewService service) =>
    Results.Ok(await service.GetFoodReviewsAsync(foodItemId)));
app.MapGet("/api/reviews/user/{userId}", async (string userId, IReviewService service) =>
    Results.Ok(await service.GetUserReviewsAsync(userId)));
app.MapPost("/api/reviews", async (ReviewDto dto, HttpContext ctx, IReviewService service) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();
    var res = await service.CreateReviewAsync(dto, userId);
    return res.Success ? Results.Created($"/api/reviews/{res.Review!.Id}", res.Review)
        : Results.BadRequest(res.Message);
});
app.MapPut("/api/reviews/{id}/approve", async (int id, IReviewService service) =>
    await service.ApproveReviewAsync(id) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/reviews/{id}", async (int id, IReviewService service) =>
    await service.DeleteReviewAsync(id) ? Results.Ok() : Results.NotFound());

// CONTACT
app.MapGet("/api/contacts", async (bool? isRead, IContactService service) =>
    Results.Ok(await service.GetAllContactsAsync(isRead)));
app.MapGet("/api/contacts/{id}", async (int id, IContactService service) =>
{
    var c = await service.GetContactByIdAsync(id);
    return c == null ? Results.NotFound() : Results.Ok(c);
});
app.MapPost("/api/contacts", async (ContactDto dto, IContactService service) =>
    Results.Created($"/api/contacts", await service.CreateContactAsync(dto)));
app.MapPut("/api/contacts/{id}/read", async (int id, IContactService service) =>
    await service.MarkAsReadAsync(id) ? Results.Ok() : Results.NotFound());
app.MapPut("/api/contacts/{id}/respond", async (int id, string response, IContactService service) =>
    await service.RespondContactAsync(id, response) ? Results.Ok() : Results.NotFound());
app.MapDelete("/api/contacts/{id}", async (int id, IContactService service) =>
    await service.DeleteContactAsync(id) ? Results.Ok() : Results.NotFound());


// SEED DB & ROLES
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    context.Database.Migrate();

    // Seed roles
    var roles = new[] { "SuperAdmin", "Admin", "QuanLyDoAn", "QuanLyNhanSu", "NhanVien", "Customer" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    // SuperAdmin
    var saEmail = "superadmin@gmail.com";
    if (await userManager.FindByEmailAsync(saEmail) == null)
    {
        var sa = new ApplicationUser { UserName = saEmail, Email = saEmail, FullName = "SuperAdmin", IsActive = true };
        var r = await userManager.CreateAsync(sa, "123123");
        if (r.Succeeded) await userManager.AddToRoleAsync(sa, "SuperAdmin");
    }
}

app.Run();
