using CabBookingMVC.Helper;
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
    name: "CabBookingList",
    pattern: "CabBookingList",
    defaults: new { controller = "Reports", action = "CabBookingList" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=Index}/{id?}");

app.Run();
