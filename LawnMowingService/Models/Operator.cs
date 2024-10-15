using System.Collections.Generic;

namespace LawnMowingService.Models
{
    public class Operator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Initialize to avoid warnings
        public int MachineId { get; set; }

        public virtual Machine Machine { get; set; } = new Machine(); // Initialize to avoid warnings
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // Initialize to avoid warnings
    }
}
