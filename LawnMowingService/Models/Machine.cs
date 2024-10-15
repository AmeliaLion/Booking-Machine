using System.Collections.Generic;

namespace LawnMowingService.Models
{
    public class Machine
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<Booking>? Bookings { get; set; } 
    }
}
