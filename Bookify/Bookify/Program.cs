using AutoMapper;
using Bookify.Data.Seeder;
using Bookify.Models;
using Bookify.Repository;
using Bookify.Repository.IRepository;
using Bookify.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;   
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT CONFIGURATION (added)
var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt.GetValue<string>("Key") ?? string.Empty;
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // optional, default is 32
    });
builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true; 
    options.TokenValidationParameters = new TokenValidationParameters 
    {
        ValidateIssuer = true, 
        ValidateAudience = true, 
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true, 
        ValidIssuer = jwt["Issuer"], 
        ValidAudience = jwt["Audience"], 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// Register TokenService
builder.Services.AddScoped<ITokenService, TokenService>(); 

// Register AutoMapper 
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly()); 

// Repository Registrations
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Auth to Swagger  
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Call seeding on startup
using (var scope = app.Services.CreateScope()) 
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); 
    dbInitializer.Initialize(); 
}

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();
app.Run();