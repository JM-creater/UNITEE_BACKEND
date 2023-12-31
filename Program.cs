using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Services;
using Microsoft.Extensions.FileProviders;
using UNITEE_BACKEND.AutoMapperConfig;
using UNITEE_BACKEND.Models.ImageDirectory;
using Microsoft.Extensions.Options;
using Hangfire;
using UNITEE_BACKEND.Models.SignalRNotification;
using UNITEE_BACKEND.Hubs;
using UNITEE_BACKEND.GenerateToken;

// Variables
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// HangFire Configuration
builder.Services.AddHangfire(config =>
   config.UseSqlServerStorage(builder.Configuration.GetConnectionString("default")));
builder.Services.AddHangfireServer();
// SignalR
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddScoped<JwtToken>();

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
                          policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .SetIsOriginAllowed((host) => true)
                                .WithMethods("OPTIONS");
                      });
});

var app = builder.Build();

var imagePathOptions = app.Services.GetRequiredService<IOptions<ImagePathOptions>>().Value;

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
app.MapHub<ChatHub>("/chathub");

app.UseHangfireDashboard();
app.UseHangfireServer();

app.UseAuthorization();

app.MapControllers();

app.Run();
