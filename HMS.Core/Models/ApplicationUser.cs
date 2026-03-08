using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Core.Models;
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
        [MaxLength(11)]
        [MinLength(11)]
        public string PersonalNumber { get; set; }

        public ICollection<Hotel> ManagedHotels { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}