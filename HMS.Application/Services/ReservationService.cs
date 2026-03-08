using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.EntityFrameworkCore;
using HMS.Application.DTO.Auth;

namespace HMS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IGenericRepository<Reservation> _reservationRepo;
        private readonly IGenericRepository<ReservationRoom> _reservationRoomRepo;

        public ReservationService(
            IGenericRepository<Reservation> reservationRepo,
            IGenericRepository<ReservationRoom> reservationRoomRepo)
        {
            _reservationRepo = reservationRepo;
            _reservationRoomRepo = reservationRoomRepo;
        }

        public async Task<Reservation> CreateBookingAsync(ReservationRequest request)
        {
            var today = DateTime.Today;

            if (request.CheckinDate.Date < today)
            {
                throw new ArgumentException("Check-in date cannot be in the past");
            }

            if (request.CheckinDate >= request.CheckoutDate)
            {
                throw new ArgumentException("Check-out date must be after Check-in date");
            }

            var overlappingBooking = await _reservationRoomRepo.Query()
                .AnyAsync(rr => request.RoomIds.Contains(rr.RoomId) &&
                                rr.Reservation!.CheckinDate < request.CheckoutDate &&
                                rr.Reservation!.CheckoutDate > request.CheckinDate);

            if (overlappingBooking)
            {
                throw new InvalidOperationException("One or more selected rooms are already occupied for these dates");
            }

            var reservation = new Reservation
            {
                CheckinDate = request.CheckinDate,
                CheckoutDate = request.CheckoutDate,
                GuestId = request.GuestId,
                ReservationRooms = request.RoomIds.Select(roomId => new ReservationRoom
                {
                    RoomId = roomId
                }).ToList()
            };

            await _reservationRepo.AddAsync(reservation);
            await _reservationRepo.SaveAsync();

            return reservation;
        }

        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId)
        {
            return await _reservationRepo.Query()
                .Include(r => r.ReservationRooms!)
                    .ThenInclude(rr => rr.Room)
                .Where(r => r.GuestId == userId)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _reservationRepo.Query()
                .Include(r => r.ReservationRooms!)
                    .ThenInclude(rr => rr.Room)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> DeleteReservationAsync(Guid id)
        {
            var reservation = await _reservationRepo.GetByIdAsync(id);
            if (reservation == null) return false;

            _reservationRepo.Delete(reservation);
            await _reservationRepo.SaveAsync();
            return true;
        }

        public async Task<Reservation> UpdateReservationDatesAsync(Guid reservationId, DateTime newCheckIn, DateTime newCheckOut)
        {
            var existing = await _reservationRepo.Query()
                .Include(r => r.ReservationRooms)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (existing == null) throw new KeyNotFoundException("Reservation not found");

            if (newCheckIn.Date < DateTime.Today) throw new ArgumentException("New dates cannot be in the past");
            if (newCheckIn.Date >= newCheckOut.Date) throw new ArgumentException("Check-out must be after check-in");

            var roomIds = existing.ReservationRooms!.Select(rr => rr.RoomId).ToList();

            var isOverlap = await _reservationRoomRepo.Query()
                .AnyAsync(rr => rr.ReservationId != reservationId &&
                                roomIds.Contains(rr.RoomId) &&
                                rr.Reservation!.CheckinDate.Date < newCheckOut.Date &&
                                rr.Reservation!.CheckoutDate.Date > newCheckIn.Date);

            if (isOverlap) throw new InvalidOperationException("New dates conflict with an existing booking");

            existing.CheckinDate = newCheckIn.Date;
            existing.CheckoutDate = newCheckOut.Date;

            _reservationRepo.Update(existing);
            await _reservationRepo.SaveAsync();

            return existing;
        }
    }
}