using Bookify.Models;

namespace Bookify.Repository.IRepository
{
    public interface IReservationRepository: IRepository<Reservation, int>
    {
        Task<IEnumerable<Reservation>> GetReservationsByUserAsync(string userId);
        Task<bool> IsRoomReservedAsync(int roomId, DateTime checkIn, DateTime checkOut);
    }
}
