using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;

namespace HMS.Application.Interfaces
{
    public interface IManagerService
{
    Task<IEnumerable<ApplicationUser>> GetManagersByHotelIdAsync(Guid hotelId);
    Task<bool> DeleteManagerAsync(string managerId);
    Task<bool> UpdateManagerAsync(ApplicationUser manager);
    Task<bool> AssignManagerToHotelAsync(string managerId, Guid hotelId); 
}
}