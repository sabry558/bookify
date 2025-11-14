namespace Bookify.Dtos.Rooms
{
    public class RoomCreateDTO
    {
        public string RoomNumber { get; set; } = string.Empty; 
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}