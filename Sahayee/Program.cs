using MongoDB.Driver;
using Sahayee.Helper;
using Sahayee.Models.DB;
using Sahayee.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var mongoDbSettings = builder.Configuration.GetSection("MongoDb");
var connectionString = mongoDbSettings.GetValue<string>("ConnectionString");
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Register MongoDbService<T> as a generic service
builder.Services.AddScoped(typeof(MongoDbService<>)); // Register MongoDbService for any T
// Add services to the container.
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/User/Login"; // Redirect to login page if unauthorized
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session expiration time
    });
// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
    new MongoClient(connectionString));

builder.Services.AddHostedService<MongoDBChangeWatcher>();

builder.Services.AddAuthorization();
// Register your models or other services
builder.Services.AddScoped<MongoDbService<User>>();
builder.Services.AddScoped<MongoDbService<Jobs>>();
builder.Services.AddScoped<MongoDbService<Course>>();
builder.Services.AddScoped<MongoDbService<JobApplication>>();
builder.Services.AddScoped<MongoDbService<CourseApplication>>();
builder.Services.AddScoped<MongoDbService<FollowUp>>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Secure session cookies
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enables serving static files from wwwroot

app.UseRouting();
app.UseSession();
// Enable authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();