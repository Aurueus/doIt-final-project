using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Auth
{
    public class ReservationRequest
    {
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public string GuestId { get; set; } = string.Empty;
        public List<Guid> RoomIds { get; set; } = new();
    }
}