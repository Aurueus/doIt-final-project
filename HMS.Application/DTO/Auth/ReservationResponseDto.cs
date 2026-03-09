using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Auth
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public string GuestId { get; set; } = string.Empty;
        public List<RoomResponseDto> Rooms { get; set; } = new();
        public string Status => CheckoutDate < DateTime.Today ? "Completed" : "Active";
    }
}