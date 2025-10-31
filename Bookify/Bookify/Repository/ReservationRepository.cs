using Bookify.Models;
using Bookify.Repository.IRepository;

namespace Bookify.Repository
{
    public class ReservationRepository: Repository<Reservation, int>, IReservationRepository
    {
        public ReservationRepository(AppDbContext _context) : base(_context) { }
    }
}
