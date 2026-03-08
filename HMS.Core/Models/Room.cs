using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HMS.Core.Models;

namespace HMS.Core.Models

{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [ForeignKey(nameof(Hotel))]
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; }
    }
}