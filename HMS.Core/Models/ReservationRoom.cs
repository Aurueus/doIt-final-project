using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Core.Models
{
    public class ReservationRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Reservation))]
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        [ForeignKey(nameof(Room))]
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    }
}