using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [Required]
        public string GuestId { get; set; } = string.Empty;

        [ForeignKey(nameof(GuestId))]
        [JsonIgnore] 
        public virtual ApplicationUser? Guest { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReservationRoom>? ReservationRooms { get; set; } = new List<ReservationRoom>();
    }
}