using AutoMapper; 
using Bookify.DTOs.Medias; 
using Bookify.DTOs.Reservations; 
using Bookify.Dtos.Rooms; 
using Bookify.DTOs.RoomTypes; 
using Bookify.Models; 
using System; 

namespace Bookify.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ----- ROOMTYPE -----
            CreateMap<RoomType, RoomTypeReadDTO>(); 
            CreateMap<RoomTypeCreateDTO, RoomType>();

            // ----- ROOM -----
            CreateMap<RoomCreateDTO, Room>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => ParseRoomNumber(src.RoomNumber)));

            CreateMap<Room, RoomReadDTO>()
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Name : string.Empty))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => $"Room {src.RoomNumber}"))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor));

            // Entity -> DTO: format room number as "Room {number}", map RoomTypeName if present
            CreateMap<Room, RoomReadDTO>() 
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Name : string.Empty)) // added
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => $"Room {src.RoomNumber}"))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Status.ToString())) 
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor)); 

            // ----- MEDIA -----
            CreateMap<Media, MediaReadDTO>()
                .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom(src => $"/api/Media/{src.Id}/file")); 

            CreateMap<MediaCreateDTO, Media>()
                .ForMember(dest => dest.Data, opt => opt.Ignore()) 
                .ForMember(dest => dest.FileName, opt => opt.Ignore()) 
                .ForMember(dest => dest.ContentType, opt => opt.Ignore());

            //----- RESERVATION -----
            CreateMap<Reservation, ReservationReadDTO>()
                 .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room != null ? $"Room {src.Room.RoomNumber}" : string.Empty)) 
                 .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty)) 
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); 

            CreateMap<ReservationCreateDTO, Reservation>()
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()) 
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatus.Pending));
        }
        private static int ParseRoomNumber(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            var cleaned = input.Replace("Room ", "", StringComparison.OrdinalIgnoreCase).Trim();
            return int.TryParse(cleaned, out int n) ? n : 0;
        }
    }
}