using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HMS.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)] 
        public string PersonalNumber { get; set; }

        public Guid? HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}