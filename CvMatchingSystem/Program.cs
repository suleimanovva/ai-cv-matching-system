using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 
using CvMatchingSystem.Data;
using CvMatchingSystem.Services;
using QuestPDF.Infrastructure; 

var builder = WebApplication.CreateBuilder(args);

// --- НАСТРОЙКА QUESTPDF ---
// Используем LicenseType, как в твоем успешном билде ранее
QuestPDF.Settings.License = LicenseType.Community; 

// --- РЕГИСТРАЦИЯ СЕРВИСОВ ---
builder.Services.AddScoped<IMatchingService, MatchingService>();

// База данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация Identity (ВАЖНО: этот метод требует пакет Microsoft.AspNetCore.Identity.UI)
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.Password.RequireDigit = false; 
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); 

var app = builder.Build();

// --- ИНИЦИАЛИЗАЦИЯ БАЗЫ (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Ошибка инициализации: " + ex.Message);
    }
}

// --- MIDDLEWARE ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Проверка "Кто ты?"
app.UseAuthorization();  // Проверка "Что можно?"

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); 

app.Run();