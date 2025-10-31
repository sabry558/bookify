using Bookify.Models;
using Bookify.Repository;
using Bookify.Repository.IRepository;

public class RoomRepository : Repository<Room, int>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) { }
}
