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
            var hotels = await _hotelRepo.GetAllAsync();
            
            var query = hotels.AsQueryable();

            if (!string.IsNullOrEmpty(country))
                query = query.Where(h => h.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrEmpty(city))
                query = query.Where(h => h.City.Contains(city, StringComparison.OrdinalIgnoreCase));

            if (rating.HasValue)
                query = query.Where(h => h.Rating == rating.Value);

            return query.ToList();
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
                existing.Name = hotel.Name;
                existing.Address = hotel.Address;
                existing.Rating = hotel.Rating;
                _hotelRepo.Update(existing);
                await _hotelRepo.SaveAsync();
            }
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            var hotel = await _hotelRepo.GetByIdAsync(id);
            if (hotel == null) return false;

            if (hotel.Rooms != null && hotel.Rooms.Any())
                return false; 

            _hotelRepo.Delete(hotel);
            await _hotelRepo.SaveAsync();
            return true;
        }
    }
}