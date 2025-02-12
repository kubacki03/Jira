using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Jira.Data;
using Jira.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("JiraContextConnection") ?? throw new InvalidOperationException("Connection string 'JiraContextConnection' not found.");;

builder.Services.AddDbContext<JiraContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<JiraUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<JiraContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();


app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();

    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
});

app.Run();