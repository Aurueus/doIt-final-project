using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;

namespace HMS.Application.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId, decimal? minPrice, decimal? maxPrice, DateTime? availabilityDate = null);
        Task<Room?> GetRoomByIdAsync(Guid roomId);
        Task AddRoomToHotelAsync(Guid hotelId, Room room);
        Task UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(Guid roomId);
    }
}