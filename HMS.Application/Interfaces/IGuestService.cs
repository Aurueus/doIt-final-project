using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;
using HMS.Application.DTO.Auth;

namespace HMS.Application.Interfaces
{
    public interface IGuestService
    {
        Task<ApplicationUser?> GetGuestByIdAsync(string guestId);
        Task<bool> UpdateGuestAsync(string guestId, GuestUpdateDto dto);
        Task<bool> DeleteGuestAsync(string guestId);
    }
}