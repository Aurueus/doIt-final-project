using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IGenericRepository<Hotel> _hotelRepo;

        public HotelService(IGenericRepository<Hotel> hotelRepo)
        {
            _hotelRepo = hotelRepo;
        }

        public async Task<IEnumerable<Hotel>> GetFilteredHotelsAsync(string? country, string? city, int? rating)
        {
            var query = _hotelRepo.Query();

            if (!string.IsNullOrEmpty(country))
                query = query.Where(h => h.Country.ToLower() == country.ToLower());

            if (!string.IsNullOrEmpty(city))
                query = query.Where(h => h.City.ToLower() == city.ToLower());

            if (rating.HasValue)
                query = query.Where(h => h.Rating == rating.Value);

            return await query.ToListAsync();
        }

        public async Task<Hotel?> GetHotelByIdAsync(Guid id)
        {
            return await _hotelRepo.GetByIdAsync(id);
        }

        public async Task CreateHotelAsync(Hotel hotel)
        {
            await _hotelRepo.AddAsync(hotel);
            await _hotelRepo.SaveAsync();
        }

        public async Task UpdateHotelAsync(Guid id, Hotel hotel)
        {
            var existing = await _hotelRepo.GetByIdAsync(id);
            if (existing != null)
            {
                if (hotel.Rating < 1 || hotel.Rating > 5)
                    throw new ArgumentException("Rating must be between 1 and 5");

                existing.Name = hotel.Name;
                existing.Address = hotel.Address;
                existing.Rating = hotel.Rating;
                existing.Country = hotel.Country;
                existing.City = hotel.City;

                _hotelRepo.Update(existing);
                await _hotelRepo.SaveAsync();
            }
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            var hotel = await _hotelRepo.Query()
                .Include(h => h.Rooms!)
                    .ThenInclude(r => r.ReservationRooms!)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null) return false;

            if (hotel.Rooms != null && hotel.Rooms.Any())
                return false;

            var hasReservations = hotel.Rooms?.Any(r => r.ReservationRooms != null && r.ReservationRooms.Any()) ?? false;
            if (hasReservations) return false;

            _hotelRepo.Delete(hotel);
            await _hotelRepo.SaveAsync();
            return true;
        }
    }
}