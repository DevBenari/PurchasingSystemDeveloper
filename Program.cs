using FastReport.Data;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Transaction.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Hubs;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("id-ID");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddSingleton<UrlMappingService>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
});

// Konfigurasi JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//Tambahan Baru
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// konfigurasi session 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Script Auto Show Login Account First Time
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "AuthCookie";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

AddScope();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaims>();

#region Areas Master Data
builder.Services.AddScoped<IUserActiveRepository>();
builder.Services.AddScoped<ISupplierRepository>();
builder.Services.AddScoped<IDiscountRepository>();
builder.Services.AddScoped<IMeasurementRepository>();
builder.Services.AddScoped<ICategoryRepository>();
builder.Services.AddScoped<ITermOfPaymentRepository>();
builder.Services.AddScoped<IBankRepository>();
builder.Services.AddScoped<IProductRepository>();
builder.Services.AddScoped<ILeadTimeRepository>();
builder.Services.AddScoped<IInitialStockRepository>();
builder.Services.AddScoped<IWarehouseLocationRepository>();
builder.Services.AddScoped<IUnitLocationRepository>();
builder.Services.AddScoped<IDepartmentRepository>();
builder.Services.AddScoped<IPositionRepository>();
builder.Services.AddScoped<IGroupRoleRepository>();
builder.Services.AddSignalR();
#endregion

#region Areas Order
builder.Services.AddScoped<IPurchaseRequestRepository>();
builder.Services.AddScoped<IApprovalRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository>();
builder.Services.AddScoped<IEmailRepository>();
builder.Services.AddScoped<IApprovalQtyDifferenceRepository>();
#endregion

#region Areas Warehouse
builder.Services.AddScoped<IReceiveOrderRepository>();
builder.Services.AddScoped<IUnitOrderRepository>();
builder.Services.AddScoped<IWarehouseTransferRepository>();
builder.Services.AddScoped<IQtyDifferenceRepository>();
#endregion

#region Areas Unit Request
builder.Services.AddScoped<IUnitRequestRepository>();
builder.Services.AddScoped<IApprovalUnitRequestRepository>();
#endregion

//Initialize Fast Report
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
var app = builder.Build();

builder.Services.AddDataProtection();

// Tambahkan middleware untuk dekripsi URL
app.UseMiddleware<DecryptUrlMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Tambahan Baru
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//   konfigurasi end session 
app.Use(async (context, next) =>
{
    var returnUrl = context.Request.Path + context.Request.QueryString;
    var loginUrl = $"/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
    var now = DateTimeOffset.UtcNow;

    if (context.Session.GetString("username") == null && context.Request.Path != loginUrl)
    {
        var username = context.User.Identity.Name;

        if (!string.IsNullOrEmpty(username))
        {
            // Middleware Session And Cookie
            UpdateDataForExpiredSession(username, app, context, returnUrl);

            context.Response.Redirect(loginUrl);
            return;
        }
    }
    else
    {
        // Jika session masih aktif, perbarui waktu "LastActivity"
        context.Session.SetString("LastActivity", DateTimeOffset.Now.ToString());
    }

    // Periksa apakah pengguna terautentikasi
    if (context.User.Identity?.IsAuthenticated == true)
    {
        // Coba ambil LastActivity dari session
        var lastActivity = context.Session.GetString("LastActivity");

        if (lastActivity == null)
        {
            // Set LastActivity pada aktivitas pertama setelah login
            context.Session.SetString("LastActivity", now.ToString("o"));
        }
        else if (DateTime.TryParse(lastActivity, out var lastActivityTime))
        {
            var sessionTimeout = TimeSpan.FromMinutes(30);

            // Jika waktu hampir habis (misalnya 15 menit sebelum habis)
            var timeRemaining = sessionTimeout - (now - lastActivityTime);
            if (timeRemaining.TotalMinutes <= 15)
            {
                // Kirim waktu yang tersisa ke klien melalui header
                context.Response.Headers["X-Session-Time-Remaining"] = timeRemaining.TotalSeconds.ToString();
            }

            // Perbarui LastActivity jika session belum habis
            if (timeRemaining.TotalSeconds > 0)
            {
                context.Session.SetString("LastActivity", now.ToString("o"));
            }
            else
            {
                // Jika session habis, lakukan logout
                var username = context.User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    // Middleware Session And Cookie
                    UpdateDataForExpiredSession(username, app, context, returnUrl);

                    context.Response.Redirect(loginUrl);
                    return;
                }
            }
        }
    }

    // Lanjutkan ke middleware berikutnya
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseFastReport();

app.UseEndpoints(endpoints =>
{
    // SignalR
    endpoints.MapHub<ChatHub>("/chathub");
    // End SignalR
    endpoints.MapDefaultControllerRoute();

    endpoints.MapControllerRoute(
        name: "MyArea",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});

app.Run();

void AddScope()
{
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
}

async void UpdateDataForExpiredSession(string username, WebApplication app, HttpContext context, string returnUrl)
{
    // Logika update data
    // Tambahkan logika lain sesuai kebutuhan, misalnya memperbarui status user di database
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

        //Cari User
        var user = dbContext.Users.FirstOrDefault(u => u.Email == username);
        if (user != null)
        {
            user.IsOnline = false;
            dbContext.SaveChanges();
        }

        // Hapus session dan sign out cookie
        context.Session.Remove("username");
        await context.SignOutAsync("CookieAuth");

        await signInManager.SignOutAsync();
        context.Response.Redirect(returnUrl);
    }
}