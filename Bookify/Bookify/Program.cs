using Bookify.Models;
using Bookify.Repository;                     
using Bookify.Repository.IRepository;         
using Microsoft.AspNetCore.Identity;          
using Microsoft.EntityFrameworkCore;
using Bookify.Data.Seeder;


using Microsoft.AspNetCore.Authentication.JwtBearer;   
using Microsoft.IdentityModel.Tokens;                  
using System.Text;                                     
using Bookify.Services;
using AutoMapper; 
using System.Reflection; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));  


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// JWT CONFIGURATION (added)
var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt.GetValue<string>("Key");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true in production
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

//Repository Registrations
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
using (var scope = app.Services.CreateScope()) //added
{ 
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); //added
    dbInitializer.Initialize(); 
} 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();