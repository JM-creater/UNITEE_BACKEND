using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Services;
using Microsoft.Extensions.FileProviders;
//using System.IO;
using UNITEE_BACKEND.AutoMapperConfig;
using UNITEE_BACKEND.Models.ImageDirectory;
using Microsoft.Extensions.Options;
using Hangfire;
using UNITEE_BACKEND.Models.SignalRNotification;
using UNITEE_BACKEND.Hubs;

// Variables
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// HangFire Configuration
builder.Services.AddHangfire(config =>
   config.UseSqlServerStorage(builder.Configuration.GetConnectionString("default")));
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// SignalR
builder.Services.AddSignalR();

// Repository Pattern
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ISizeQuantityService, SizeQuantityService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// DbContext Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

// Image Path
builder.Services.Configure<ImagePathOptions>(builder.Configuration.GetSection("ImagePath"));

// Json Serializer
builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfigProfile));

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5173", "https://127.0.0.1:5173")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .SetIsOriginAllowed((host) => true)
                                .WithMethods("OPTIONS");
                      });
});

builder.Services.AddDistributedMemoryCache();

// Cookie
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

var imagePathOptions = app.Services.GetRequiredService<IOptions<ImagePathOptions>>().Value;

app.UseSession();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Local Image Directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.PathImages)),
    RequestPath = $"/{imagePathOptions.PathImages}"
});

app.UseCors(MyAllowSpecificOrigins);

app.MapHub<NotificationHub>("/notificationHub");

app.UseHangfireDashboard();
app.UseHangfireServer();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
