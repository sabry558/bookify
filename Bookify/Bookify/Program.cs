using Bookify.Models;
using Bookify.Repository;                     
using Bookify.Repository.IRepository;         
using Microsoft.AspNetCore.Identity;          
using Microsoft.EntityFrameworkCore;
using Bookify.Data.Seeder;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));  


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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
builder.Services.AddSwaggerGen();             

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
app.UseAuthorization();
app.MapControllers();
app.Run();