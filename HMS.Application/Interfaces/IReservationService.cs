using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;
using HMS.Application.DTO.Auth;

namespace HMS.Application.Interfaces
{
    public interface IReservationService
    {
        Task<Reservation> CreateBookingAsync(ReservationRequest request);
        Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId);
        Task<Reservation?> GetByIdAsync(Guid id);
        Task<bool> DeleteReservationAsync(Guid id);
        Task<Reservation> UpdateReservationDatesAsync(Guid reservationId, DateTime newCheckIn, DateTime newCheckOut);
    }
}