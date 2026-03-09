using AutoMapper;
using HMS.Application.DTO.Auth;
using HMS.Core.Models;

namespace HMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Hotel, HotelResponseDto>();
            CreateMap<HotelCreateDto, Hotel>();
            CreateMap<HotelUpdateDto, Hotel>();

            CreateMap<Room, RoomResponseDto>()
                .ForMember(d => d.Price, o => o.MapFrom(s => (decimal)s.Price));
            CreateMap<RoomCreateDto, Room>()
                .ForMember(d => d.Price, o => o.MapFrom(s => (double)s.Price));
            CreateMap<RoomUpdateDto, Room>()
                .ForMember(d => d.Price, o => o.MapFrom(s => (double)s.Price));
            CreateMap<ApplicationUser, ManagerResponseDto>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email ?? string.Empty));
                
            CreateMap<ApplicationUser, GuestResponseDto>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email ?? string.Empty));

            CreateMap<Reservation, ReservationResponseDto>()
                .ForMember(d => d.Rooms, o => o.MapFrom(s =>
                    s.ReservationRooms != null
                        ? s.ReservationRooms.Where(rr => rr.Room != null).Select(rr => rr.Room).ToList()
                        : new List<Room>()));
        }
    }
}