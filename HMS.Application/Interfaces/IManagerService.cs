using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;
using HMS.Application.DTO.Auth;

namespace HMS.Application.Interfaces
{
    public interface IManagerService
    {
        Task<IEnumerable<ApplicationUser>> GetManagersByHotelIdAsync(Guid hotelId);
        Task<bool> AssignManagerToHotelAsync(string managerId, Guid hotelId);
        Task<bool> UpdateManagerAsync(string managerId, ManagerUpdateDto dto);
        Task<bool> DeleteManagerAsync(string managerId);
    }
}