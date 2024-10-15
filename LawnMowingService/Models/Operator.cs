using System.Collections.Generic;

namespace LawnMowingService.Models
{
    public class Operator
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MachineId { get; set; }

        public Machine Machine { get; set; } = new Machine();
        public ICollection<Booking> Bookings { get; set; }
    }
}
