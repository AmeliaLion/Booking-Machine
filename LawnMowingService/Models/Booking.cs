using System;

namespace LawnMowingService.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public int MachineId { get; set; }
        public int? OperatorId { get; set; } // This is the ID of the assigned operator
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Pending";

        public virtual Customer Customer { get; set; } = new Customer();
        public virtual Machine Machine { get; set; } = new Machine();
        public virtual Operator Operator { get; set; } = new Operator();

        // Optionally, if you need to have a distinct property for the assigned operator ID
        // public int? AssignedOperatorId => OperatorId; // If you want to have a separate property
    }
}
