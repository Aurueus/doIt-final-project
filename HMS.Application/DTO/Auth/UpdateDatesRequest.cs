using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Auth
{
    public class UpdateDatesRequest
    {
        public DateTime NewCheckIn { get; set; }
        public DateTime NewCheckOut { get; set; }
    }
}