using Bookify.Models;

public class Media
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public MediaType MediaType { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
    public RoomType RoomType { get; set; }
}