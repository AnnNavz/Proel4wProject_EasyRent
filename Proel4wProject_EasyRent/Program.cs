using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Proel4wProject_EasyRent.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Proel4wProject_EasyRentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Proel4wProject_EasyRentContext") ?? throw new InvalidOperationException("Connection string 'Proel4wProject_EasyRentContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("CookieAuth")
	.AddCookie("CookieAuth", config =>
	{
		config.Cookie.Name = "UserLoginCookie";
		config.LoginPath = "/Account/Login"; // Where to go if not authorized
	});

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LoginView}/{id?}");

app.Run();
