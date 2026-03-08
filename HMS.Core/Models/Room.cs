using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace HMS.Core.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; 

        [Required]
        public double Price { get; set; }

        [Required] 
        public Guid HotelId { get; set; }

        [JsonIgnore]
        public virtual Hotel? Hotel { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReservationRoom>? ReservationRooms { get; set; } = new List<ReservationRoom>();
    }
}