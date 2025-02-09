using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Jira.Data;
using Jira.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("JiraContextConnection") ?? throw new InvalidOperationException("Connection string 'JiraContextConnection' not found.");;

builder.Services.AddDbContext<JiraContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<JiraUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<JiraContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication(); // Ensure this is added before authorization
app.MapRazorPages(); // Dodaje obs³ugê stron Identity
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();

    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
});
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
