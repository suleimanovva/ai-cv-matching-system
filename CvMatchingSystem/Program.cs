using Microsoft.EntityFrameworkCore;
using CvMatchingSystem.Data;
using CvMatchingSystem.Services;
// ДОБАВЛЕНО: Для работы с инфраструктурой QuestPDF
using QuestPDF.Infrastructure; 

var builder = WebApplication.CreateBuilder(args);

// --- НАСТРОЙКА ЛИЦЕНЗИИ (ОБЯЗАТЕЛЬНО ПО ТЗ) ---
// Мы используем Community, так как это учебный проект. 
// Это уберет ту самую ошибку ValidateLicense().
QuestPDF.Settings.License = LicenseType.Community; 
// ----------------------------------------------

builder.Services.AddScoped<IMatchingService, MatchingService>();

// Подключаем базу данных SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();