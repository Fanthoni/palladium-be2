var builder = WebApplication.CreateBuilder(args);

AppConfig.LoadConfiguration();

builder.Services.AddControllersWithViews();
var app = builder.Build();

app.MapGet("/", () => "Hello Palladium API!");


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("PORT"));
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