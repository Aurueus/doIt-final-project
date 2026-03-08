using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models; 

namespace HMS.Application.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetFilteredHotelsAsync(string? country, string? city, int? rating);
        Task<Hotel?> GetHotelByIdAsync(Guid id);
        Task CreateHotelAsync(Hotel hotel);
        Task UpdateHotelAsync(Guid id, Hotel hotel);
        Task<bool> DeleteHotelAsync(Guid id); 
    }
}