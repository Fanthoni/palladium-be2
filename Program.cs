using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

AppConfig.LoadConfiguration();

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
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5206";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");    
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