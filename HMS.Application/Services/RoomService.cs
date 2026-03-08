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

        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(
            Guid hotelId,
            decimal? minPrice,
            decimal? maxPrice,
            DateTime? availabilityDate = null)
        {
            var query = _roomRepo.Query().Where(r => r.HotelId == hotelId);

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= (double)minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= (double)maxPrice.Value);

            if (availabilityDate.HasValue)
            {
                query = query.Where(r => !r.ReservationRooms.Any(rr =>
                    availabilityDate.Value >= rr.Reservation.CheckinDate &&
                    availabilityDate.Value < rr.Reservation.CheckoutDate));
            }

            return await query.ToListAsync();
        }

        public async Task AddRoomToHotelAsync(Guid hotelId, Room room)
        {
            if (room.Price <= 0)
                throw new ArgumentException("Price must be greater than 0");

            room.HotelId = hotelId;
            await _roomRepo.AddAsync(room);
            await _roomRepo.SaveAsync();
        }

        public async Task<bool> DeleteRoomAsync(Guid roomId)
        {
            var room = await _roomRepo.Query()
                .Include(r => r.ReservationRooms!)
                    .ThenInclude(rr => rr.Reservation)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) return false;

            var hasActiveOrFutureBookings = room.ReservationRooms?
                .Any(rr => rr.Reservation != null && rr.Reservation.CheckoutDate >= DateTime.Today) ?? false;

            if (hasActiveOrFutureBookings) return false;

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
            var existingRoom = await _roomRepo.GetByIdAsync(room.Id);

            if (existingRoom != null)
            {
                if (room.Price <= 0)
                    throw new ArgumentException("Price must be greater than 0");

                existingRoom.Name = room.Name;
                existingRoom.Price = room.Price;

                _roomRepo.Update(existingRoom);
                await _roomRepo.SaveAsync();
            }
        }
    }
}