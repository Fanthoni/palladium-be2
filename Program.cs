using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

AppConfig.LoadConfiguration();

// Determine the port from environment or use default 5206.
var port = Environment.GetEnvironmentVariable("PORT") ?? "5206";

// Configure the web host to listen on all interfaces.
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins(AppConfig.AllowedOrigin)
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.MapGet("/", () => "Hello Palladium API!");

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