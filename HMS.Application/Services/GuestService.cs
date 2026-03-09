using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GuestService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetGuestByIdAsync(string guestId)
        {
            return await _userManager.FindByIdAsync(guestId);
        }

        public async Task<bool> UpdateGuestAsync(string guestId, GuestUpdateDto dto)
        {
            var existing = await _userManager.FindByIdAsync(guestId);
            if (existing == null) return false;

            var phoneExists = await _userManager.Users
                .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber && u.Id != guestId);
            if (phoneExists)
                throw new ArgumentException("Phone number is already in use.");

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(existing);
            return result.Succeeded;
        }

        public async Task<bool> DeleteGuestAsync(string guestId)
        {
            var guest = await _userManager.Users
                .Include(u => u.Reservations)
                .FirstOrDefaultAsync(u => u.Id == guestId);

            if (guest == null) return false;

            var hasFutureBookings = guest.Reservations?
                .Any(r => r.CheckoutDate >= DateTime.Today) ?? false;

            if (hasFutureBookings)
                throw new InvalidOperationException("Cannot delete guest: they have active or future reservations.");

            var result = await _userManager.DeleteAsync(guest);
            return result.Succeeded;
        }
    }
}