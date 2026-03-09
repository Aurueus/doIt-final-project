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
        Task<Reservation> CreateBookingAsync(ReservationCreateDto dto, string callerId, Guid hotelId);
        Task<Reservation?> GetByIdAsync(Guid id);
        Task<IEnumerable<Reservation>> SearchAsync(string? guestId, Guid? hotelId, Guid? roomId, DateTime? date, bool? activeOnly);
        Task<Reservation> UpdateReservationDatesAsync(Guid reservationId, DateTime newCheckIn, DateTime newCheckOut, string callerId, bool isAdmin);
        Task<bool> DeleteReservationAsync(Guid id, string callerId, bool isAdmin);
    }
}