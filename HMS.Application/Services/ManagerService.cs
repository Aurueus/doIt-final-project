using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetManagersByHotelIdAsync(Guid hotelId)
        {
            return await _userManager.Users
                .Where(u => u.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<bool> AssignManagerToHotelAsync(string managerId, Guid hotelId)
        {
            var manager = await _userManager.FindByIdAsync(managerId);
            if (manager == null) return false;

            manager.HotelId = hotelId;
            var result = await _userManager.UpdateAsync(manager);
            return result.Succeeded;
        }

        public async Task<bool> UpdateManagerAsync(string managerId, ManagerUpdateDto dto)
        {
            var existing = await _userManager.FindByIdAsync(managerId);
            if (existing == null) return false;

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(existing);
            return result.Succeeded;
        }

        public async Task<bool> DeleteManagerAsync(string managerId)
        {
            var manager = await _userManager.FindByIdAsync(managerId);
            if (manager == null || !manager.HotelId.HasValue) return false;

            var otherManagersCount = await _userManager.Users
                .CountAsync(u => u.HotelId == manager.HotelId && u.Id != managerId);

            if (otherManagersCount == 0)
                throw new InvalidOperationException("Cannot delete: this is the last manager of the hotel.");

            var result = await _userManager.DeleteAsync(manager);
            return result.Succeeded;
        }
    }
}