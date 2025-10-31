using Bookify.Models;
using Bookify.Repository.IRepository;

namespace Bookify.Repository
{
    public class PaymentRepository: Repository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }
    }
}
