using AutoMapper;

using Bookify.DTOs.RoomTypes;
using Bookify.DTOs.Medias;
using Bookify.DTOs.Rooms;   
using Bookify.Models;

namespace Bookify.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ----- ROOMTYPE -----
            CreateMap<RoomType, RoomTypeReadDTO>();
            CreateMap<RoomTypeCreateDTO, RoomType>();

            // ----- ROOM ----- ✅ (this was missing, now added)
            CreateMap<RoomCreateDTO, Room>();
            CreateMap<Room, RoomReadDTO>();

            // ----- MEDIA -----
            CreateMap<Media, MediaReadDTO>()
                .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom(src => $"/api/Media/{src.Id}/file"));

            CreateMap<MediaCreateDTO, Media>()
                .ForMember(dest => dest.Data, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.Ignore())
                .ForMember(dest => dest.ContentType, opt => opt.Ignore());
        }
    }
}