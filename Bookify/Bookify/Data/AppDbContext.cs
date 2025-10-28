
using Bookify.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<ApplicationUser> Users;
    public DbSet<Media> medias;
    public DbSet<Payment> Payments;
    public DbSet<Room> Rooms;
    public DbSet<RoomType> RoomTypes;
    public DbSet<Reservation> Reservations;


}
