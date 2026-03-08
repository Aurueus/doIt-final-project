using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using HMS.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Core.Models

{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime CheckinDate { get; set; }

        [Required]
        public DateTime CheckoutDate { get; set; }

        [ForeignKey(nameof(Guest))]
        public string GuestId { get; set; }
        public ApplicationUser Guest { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; }
    }
}