using FBS.Application.Services.Interfaces;
using FBS.Application.Services;
using FBS.Infrastructure.Configuration;
using FBS.Infrastructure.DataAccess.Seed;
using FBS.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using FBS.Infrastructure.Entities;
using FBS.DataAccess.Contexts;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Product.Create", policy => policy.RequireClaim("Permission", "Product.Create"));
    options.AddPolicy("Product.Edit", policy => policy.RequireClaim("Permission", "Product.Edit"));
    options.AddPolicy("Product.Delete", policy => policy.RequireClaim("Permission", "Product.Delete"));
    options.AddPolicy("Category.Create", policy => policy.RequireClaim("Permission", "Category.Create"));
    options.AddPolicy("Category.Edit", policy => policy.RequireClaim("Permission", "Category.Edit"));
    options.AddPolicy("Category.Delete", policy => policy.RequireClaim("Permission", "Category.Delete"));

    
    options.AddPolicy("Customer.Create", policy => policy.RequireClaim("Permission", "Customer.Create"));
    options.AddPolicy("Customer.Edit", policy => policy.RequireClaim("Permission", "Customer.Edit"));
    options.AddPolicy("Customer.Update", policy => policy.RequireClaim("Permission", "Customer.Update"));
    options.AddPolicy("Customer.Delete", policy => policy.RequireClaim("Permission", "Customer.Delete"));
    options.AddPolicy("Contact.Manage", policy => policy.RequireClaim("Permission", "Contact.Manage"));
    options.AddPolicy("Contact.Delete", policy => policy.RequireClaim("Permission", "Contact.Delete "));

    options.AddPolicy("Order.View", policy => policy.RequireClaim("Permission", "Order.View"));
    options.AddPolicy("Order.Update", policy => policy.RequireClaim("Permission", "Order.Update"));
    options.AddPolicy("Order.ConfirmPayment", policy => policy.RequireClaim("Permission", "Order.ConfirmPayment"));

    options.AddPolicy("Blog.Create", policy => policy.RequireClaim("Permission", "Blog.Create"));
    options.AddPolicy("Blog.Edit", policy => policy.RequireClaim("Permission", "Blog.Edit"));
    options.AddPolicy("Blog.Delete", policy => policy.RequireClaim("Permission", "Blog.Delete"));
    options.AddPolicy("Review.Manage", policy => policy.RequireClaim("Permission", "Review.Manage"));

    options.AddPolicy("Revenue.View", policy => policy.RequireClaim("Permission", "Revenue.View"));
});


builder.Services.SetDBContext(builder.Configuration);


builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();


builder.Services.InitialApplicationServices(builder.Configuration);


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Auth/Login";
    options.AccessDeniedPath = "/Admin/Auth/Login";
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddHttpClient<VietQRService>();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAndAdminAsync(services);
    services.InitialDBAndSeedData();
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}"
    );

 
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
