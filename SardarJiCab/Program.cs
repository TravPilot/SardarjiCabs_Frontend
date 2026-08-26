using CabBookingMVC.Helper;
<<<<<<< HEAD
using Microsoft.AspNetCore.Connections;
using Rotativa.AspNetCore;
=======
>>>>>>> parent of b370c3a (commit chage)
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
<<<<<<< HEAD
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IAddPageRepository, AddPageRepository>();
=======
>>>>>>> parent of b370c3a (commit chage)

builder.Services.AddScoped<IDriverDashboardDB, DriverDashboardDB>();
builder.Services.AddScoped<IDriverDashboardBL, DriverDashboardBL>();

builder.Services.AddScoped<IDriverTripsDB, DriverTripsDB>();
builder.Services.AddScoped<IDriverTripsBL, DriverTripsBL>();

builder.Services.AddScoped<IDriverLoginDB, DriverLoginDB>();
builder.Services.AddScoped<IDriverLoginBL, DriverLoginBL>();

builder.Services.AddScoped<IDriverProfileDB, DriverProfileDB>();
builder.Services.AddScoped<IDriverProfileBL, DriverProfileBL>();

builder.Services.AddScoped<IDriverEarningsDB, DriverEarningsDB>();
builder.Services.AddScoped<IDriverEarningsBL, DriverEarningsBL>();

builder.Services.AddScoped<IDriverDocumentsDB, DriverDocumentsDB>();
builder.Services.AddScoped<IDriverDocumentsBL, DriverDocumentsBL>();

builder.Services.AddScoped<IDriverSupportDB, DriverSupportDB>();
builder.Services.AddScoped<IDriverSupportBL, DriverSupportBL>();

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
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");
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
    name: "CabBookingList",
    pattern: "CabBookingList",
    defaults: new { controller = "Reports", action = "CabBookingList" });
<<<<<<< HEAD

app.MapControllerRoute(
    name: "Noservice",
    pattern: "Noservice",
    defaults: new { controller = "Home", action = "CabnotAbaiable" });

app.MapControllerRoute(
    name: "SupportReview",
    pattern: "SupportReview",
    defaults: new { controller = "Reports", action = "SupportReview" });

app.MapControllerRoute(
    name: "forgotpassword",
    pattern: "forgotpassword",
    defaults: new { controller = "Customer", action = "ForgotPassword" });
app.MapControllerRoute(
    name: "premiumsupport",
    pattern: "premiumsupport",
    defaults: new { controller = "Customer", action = "Contactus" });
=======
>>>>>>> parent of b370c3a (commit chage)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=Index}/{id?}");

app.Run();
