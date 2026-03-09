using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IGenericRepository<Reservation> _reservationRepo;
        private readonly IGenericRepository<ReservationRoom> _reservationRoomRepo;
        private readonly IGenericRepository<Room> _roomRepo;

        public ReservationService(
            IGenericRepository<Reservation> reservationRepo,
            IGenericRepository<ReservationRoom> reservationRoomRepo,
            IGenericRepository<Room> roomRepo)
        {
            _reservationRepo = reservationRepo;
            _reservationRoomRepo = reservationRoomRepo;
            _roomRepo = roomRepo;
        }

        public async Task<Reservation> CreateBookingAsync(ReservationCreateDto dto, string callerId, Guid hotelId)
        {
            if (dto.CheckinDate.Date < DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (dto.CheckinDate >= dto.CheckoutDate)
                throw new ArgumentException("Check-out date must be after check-in date.");

            var rooms = await _roomRepo.Query()
                .Where(r => dto.RoomIds.Contains(r.Id))
                .ToListAsync();

            if (rooms.Count != dto.RoomIds.Count)
                throw new ArgumentException("One or more room IDs are invalid.");

            if (rooms.Any(r => r.HotelId != hotelId))
                throw new ArgumentException("All rooms must belong to the specified hotel.");

            var hasOverlap = await _reservationRoomRepo.Query()
                .AnyAsync(rr =>
                    dto.RoomIds.Contains(rr.RoomId) &&
                    rr.Reservation!.CheckinDate < dto.CheckoutDate &&
                    rr.Reservation!.CheckoutDate > dto.CheckinDate);

            if (hasOverlap)
                throw new InvalidOperationException("One or more selected rooms are already booked for these dates.");

            var reservation = new Reservation
            {
                CheckinDate = dto.CheckinDate,
                CheckoutDate = dto.CheckoutDate,
                GuestId = callerId,
                ReservationRooms = dto.RoomIds.Select(roomId => new ReservationRoom
                {
                    RoomId = roomId
                }).ToList()
            };

            await _reservationRepo.AddAsync(reservation);
            await _reservationRepo.SaveAsync();

            return reservation;
        }

        public async Task<Reservation?> GetByIdAsync(Guid id)
        {
            return await _reservationRepo.Query()
                .Include(r => r.ReservationRooms!)
                    .ThenInclude(rr => rr.Room)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> SearchAsync(
            string? guestId,
            Guid? hotelId,
            Guid? roomId,
            DateTime? date,
            bool? activeOnly)
        {
            IQueryable<Reservation> query = _reservationRepo.Query()
                .Include(r => r.ReservationRooms!)
                    .ThenInclude(rr => rr.Room);

            if (!string.IsNullOrEmpty(guestId))
                query = query.Where(r => r.GuestId == guestId);

            if (hotelId.HasValue)
                query = query.Where(r =>
                    r.ReservationRooms!.Any(rr => rr.Room!.HotelId == hotelId.Value));

            if (roomId.HasValue)
                query = query.Where(r =>
                    r.ReservationRooms!.Any(rr => rr.RoomId == roomId.Value));

            if (date.HasValue)
                query = query.Where(r =>
                    r.CheckinDate.Date <= date.Value.Date &&
                    r.CheckoutDate.Date > date.Value.Date);

            if (activeOnly.HasValue)
            {
                if (activeOnly.Value)
                    query = query.Where(r => r.CheckoutDate >= DateTime.Today);
                else
                    query = query.Where(r => r.CheckoutDate < DateTime.Today);
            }

            return await query.ToListAsync();
        }

        public async Task<Reservation> UpdateReservationDatesAsync(
            Guid reservationId,
            DateTime newCheckIn,
            DateTime newCheckOut,
            string callerId,
            bool isAdmin)
        {
            var existing = await _reservationRepo.Query()
                .Include(r => r.ReservationRooms)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (existing == null)
                throw new KeyNotFoundException("Reservation not found.");

            if (!isAdmin && existing.GuestId != callerId)
                throw new UnauthorizedAccessException("You can only modify your own reservations.");

            if (newCheckIn.Date < DateTime.Today)
                throw new ArgumentException("New check-in date cannot be in the past.");

            if (newCheckIn.Date >= newCheckOut.Date)
                throw new ArgumentException("Check-out must be after check-in.");

            var roomIds = existing.ReservationRooms!.Select(rr => rr.RoomId).ToList();

            var hasOverlap = await _reservationRoomRepo.Query()
                .AnyAsync(rr =>
                    rr.ReservationId != reservationId &&
                    roomIds.Contains(rr.RoomId) &&
                    rr.Reservation!.CheckinDate.Date < newCheckOut.Date &&
                    rr.Reservation!.CheckoutDate.Date > newCheckIn.Date);

            if (hasOverlap)
                throw new InvalidOperationException("New dates conflict with an existing booking.");

            existing.CheckinDate = newCheckIn.Date;
            existing.CheckoutDate = newCheckOut.Date;

            _reservationRepo.Update(existing);
            await _reservationRepo.SaveAsync();

            return existing;
        }

        public async Task<bool> DeleteReservationAsync(Guid id, string callerId, bool isAdmin)
        {
            var reservation = await _reservationRepo.GetByIdAsync(id);
            if (reservation == null) return false;

            if (!isAdmin && reservation.GuestId != callerId)
                throw new UnauthorizedAccessException("You can only cancel your own reservations.");

            _reservationRepo.Delete(reservation);
            await _reservationRepo.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(string guestId)
        {
            return await _reservationRepo.Query()
                .Include(r => r.ReservationRooms!)
                    .ThenInclude(rr => rr.Room)
                .Where(r => r.GuestId == guestId)
                .ToListAsync();
        }
    }
}