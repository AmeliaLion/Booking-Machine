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

        public Customer? Customer { get; set; }
        public Machine? Machine { get; set; }
        public Operator? Operator { get; set; }

        // Optionally, if you need to have a distinct property for the assigned operator ID
        // public int? AssignedOperatorId => OperatorId; // If you want to have a separate property
    }
}
