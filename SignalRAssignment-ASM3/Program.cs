using Microsoft.EntityFrameworkCore;
using SignalRAssignment_ASM3.Hubs;
using SignalRAssignment_ASM3.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResponseCaching(options =>
{
    options.UseCaseSensitivePaths = false;
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddDbContext<PostsManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAuthentication("PostAppSaintRai")
    .AddCookie("PostAppSaintRai", options =>
    {
        options.LoginPath = "/Members/Login";
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StaffPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Staff"));
    options.AddPolicy("UserPolicy", policy =>
            policy.RequireClaim(ClaimTypes.Role, "User"));
});


builder.Services.AddSignalR();

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


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<PostHub<string>>("/PostHub/string");
app.MapHub<PostHub<Post>>("/PostHub/post");

app.Run();
