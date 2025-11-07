using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description {  get; set; }
        [Required]
        public decimal Price {  get; set; }
        public int Capacity {  get; set; }

        public ICollection<Room> Rooms { get; set; }
        public ICollection<Media> Medias { get; set; }



    }
}
