using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomRepo;

        public RoomService(IGenericRepository<Room> roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId, decimal? minPrice, decimal? maxPrice)
        {
            var rooms = await _roomRepo.GetAllAsync();
            var query = rooms.Where(r => r.HotelId == hotelId).AsQueryable();

            if (minPrice.HasValue) query = query.Where(r => r.Price >= (double)minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(r => r.Price <= (double)maxPrice.Value);
            return query.ToList();
        }

        public async Task AddRoomToHotelAsync(Guid hotelId, Room room)
        {
            if (room.Price <= 0) throw new ArgumentException("Price must be greater than 0.");

            room.HotelId = hotelId;
            await _roomRepo.AddAsync(room);
            await _roomRepo.SaveAsync();
        }

        public async Task<bool> DeleteRoomAsync(Guid roomId)
        {
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room == null) return false;

            if (room.ReservationRooms != null && room.ReservationRooms.Any())
                return false;

            _roomRepo.Delete(room);
            await _roomRepo.SaveAsync();
            return true;
        }

        public async Task<Room?> GetRoomByIdAsync(Guid roomId)
        {
            return await _roomRepo.GetByIdAsync(roomId);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _roomRepo.Update(room);
            await _roomRepo.SaveAsync();
        }
    }
}