using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> UpdateGuestAsync(string guestId, ApplicationUser updatedGuest)
        {
            var existing = await _userManager.FindByIdAsync(guestId);
            if (existing == null) return false;

            var phoneExists = await _userManager.Users
                .AnyAsync(u => u.PhoneNumber == updatedGuest.PhoneNumber && u.Id != guestId);
            if (phoneExists) throw new ArgumentException("Phone number is already in use");

            existing.FirstName = updatedGuest.FirstName;
            existing.LastName = updatedGuest.LastName;
            existing.PhoneNumber = updatedGuest.PhoneNumber;

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

            if (hasFutureBookings) return false;

            var result = await _userManager.DeleteAsync(guest);
            return result.Succeeded;
        }
    }
}