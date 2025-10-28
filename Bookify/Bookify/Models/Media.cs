using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Models
{
    public class Media
    {
        public int Id { get; set; }
        [Required]
        public string MediaType { get; set; }
        [Required]
        public string Link {  get; set; }

        public int RoomTypeId {  get; set; }
        [ForeignKey("RoomTypeId")]
        public RoomType RoomType { get; set; }  
    }
}
