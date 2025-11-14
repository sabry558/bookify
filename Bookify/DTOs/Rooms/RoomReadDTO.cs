namespace Bookify.Dtos.Rooms
{
    public class RoomReadDTO
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty; 
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }

        public int Floor { get; set; }
    }
}