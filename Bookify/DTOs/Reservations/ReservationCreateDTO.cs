using Bookify.Models;

namespace Bookify.DTOs.Reservations
{
    public class ReservationCreateDTO
    {
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal Amount { get; set; }     // total price
        public PaymentTypeEnum PaymentType { get; set; }
    }
}