using Bookify.Models;
using Bookify.Repository.IRepository;

namespace Bookify.Repository
{
    public class RoomTypeRepository : Repository<RoomType, int>, IRoomTypeRepository
    {
        public RoomTypeRepository(AppDbContext context) : base(context) { }
    }
}
