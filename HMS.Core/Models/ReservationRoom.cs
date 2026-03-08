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
    public Guid Id { get; set; } 

    public Guid ReservationId { get; set; }
    public virtual Reservation? Reservation { get; set; }

    public Guid RoomId { get; set; }
    public virtual Room? Room { get; set; }
}
}