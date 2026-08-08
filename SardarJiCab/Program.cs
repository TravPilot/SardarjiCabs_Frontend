using CabBookingMVC.Helper;
using Microsoft.AspNetCore.Connections;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarjiCab.DB;
using SardarjiCab.DB.Interface;
using SardarJiCab.BL;
using SardarJiCab.BL.Interface;
using SardarJiCab.Model;
using SardarJiCab.Services;
using SardarJiCab.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ILogInService, LogInService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<PaymentGatwaySettings>();
builder.Services.AddScoped<IRazorGatewayRepository, RazorGatewayRepository>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

builder.Services.AddScoped<ICouponRepository, CouponRepository>();

builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});
builder.Services.AddScoped<IDriverDashboardDB, DriverDashboardDB>();
builder.Services.AddScoped<IDriverDashboardBL, DriverDashboardBL>();

builder.Services.AddScoped<IDriverTripsDB, DriverTripsDB>();
builder.Services.AddScoped<IDriverTripsBL, DriverTripsBL>();

builder.Services.AddScoped<IDriverLoginDB, DriverLoginDB>();
builder.Services.AddScoped<IDriverLoginBL, DriverLoginBL>();
builder.Services.AddMemoryCache();
//builder.Services.AddScoped<ISmsSender, YourSmsSenderImplementation>();

builder.Services.Configure<BrevoSmtpOptions>(builder.Configuration.GetSection("BrevoSmtp"));
builder.Services.AddScoped<IEmailSender, BrevoSmtpEmailSender>();

builder.Services.AddControllersWithViews();

// Register SignalR
builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddHttpClient();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "CurrentRide",
    pattern: "CurrentRide",
    defaults: new { controller = "Booking", action = "CurrentRide" });
app.MapControllerRoute(
    name: "profile",
    pattern: "profile",
    defaults: new { controller = "Customer", action = "Index" });

app.MapControllerRoute(
    name: "CabBookingList",
    pattern: "CabBookingList",
    defaults: new { controller = "Reports", action = "CabBookingList" });

app.MapControllerRoute(
    name: "forgotpassword",
    pattern: "forgotpassword",
    defaults: new { controller = "Customer", action = "ForgotPassword" });
app.MapControllerRoute(
    name: "premiumsupport",
    pattern: "premiumsupport",
    defaults: new { controller = "Customer", action = "Contactus" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=Index}/{id?}");

app.Run();
