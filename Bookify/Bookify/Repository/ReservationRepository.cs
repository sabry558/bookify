using Bookify.Models;
using Bookify.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bookify.Repository
{
    public class ReservationRepository : Repository<Reservation, int>, IReservationRepository
    {
        public ReservationRepository(AppDbContext context) : base(context) { }

       public async Task<IEnumerable<Reservation>> GetReservationsByUserAsync(string userId)
       {
           return await dbSet.Where(r => r.UserId == userId)
                             .Include(r => r.Room)
                             .ThenInclude(rt => rt.RoomType)
                             .ToListAsync();
       }

       public async Task<bool> IsRoomReservedAsync(int roomId, DateTime checkIn, DateTime checkOut)
       {
           return await dbSet.AnyAsync(r =>
               r.RoomId == roomId &&
               r.CheckIn<checkOut &&
               r.CheckOut> checkIn);
       }
    }
}