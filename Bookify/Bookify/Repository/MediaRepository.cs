using Bookify.Models;
using Bookify.Repository.IRepository;

namespace Bookify.Repository
{
    public class MediaRepository: Repository<Media, int>, IMediaRepository
    {
        public MediaRepository(AppDbContext context) : base(context) { }
    }
}
