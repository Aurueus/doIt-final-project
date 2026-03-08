using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;

namespace HMS.Application.Interfaces
{
    public interface IGuestService
    {
        Task<ApplicationUser?> GetGuestByIdAsync(string guestId);
        Task<bool> UpdateGuestAsync(string guestId, ApplicationUser updatedGuest);
        Task<bool> DeleteGuestAsync(string guestId);
    }
}