namespace Bookify.DTOs.Rooms
{
    public class RoomReadDTO   
    {
        public int Id { get; set; }  
        public string Name { get; set; }  
        public int RoomTypeId { get; set; }  

        public string RoomTypeName { get; set; }  
    }
}