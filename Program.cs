using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Services;
using Microsoft.Extensions.FileProviders;
//using System.IO;
using UNITEE_BACKEND.AutoMapperConfig;
using UNITEE_BACKEND.Models.ImageDirectory;
using Microsoft.Extensions.Options;
using Hangfire;

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

// Repository Pattern
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ISizeQuantityService, SizeQuantityService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
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
                                .SetIsOriginAllowed((host) => true)
                                .AllowCredentials()
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
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.ProductImages)),
    RequestPath = $"/{imagePathOptions.ProductImages}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.Images)),
    RequestPath = $"/{imagePathOptions.Images}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.SupplierImage)),
    RequestPath = $"/{imagePathOptions.SupplierImage}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.StudyLoad)),
    RequestPath = $"/{imagePathOptions.StudyLoad}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.BIR)),
    RequestPath = $"/{imagePathOptions.BIR}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.CityPermit)),
    RequestPath = $"/{imagePathOptions.CityPermit}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.SchoolPermit)),
    RequestPath = $"/{imagePathOptions.SchoolPermit}"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), imagePathOptions.ProofOfPayment)),
    RequestPath = $"/{imagePathOptions.ProofOfPayment}"
});

app.UseCors(MyAllowSpecificOrigins);

app.UseHangfireDashboard();
app.UseHangfireServer();

app.UseAuthorization();

app.MapControllers();

app.Run();
