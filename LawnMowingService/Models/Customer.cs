using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LawnMowingService.Models // Ensure this matches your project structure
{
    public class Customer : IdentityUser // Inherit from IdentityUser
    {
        public new string Id { get; set; } = Guid.NewGuid().ToString(); // Use 'new' to hide the inherited member
        public string Name { get; set; } = string.Empty; // Initialize to avoid warnings
        public ICollection<Booking> Bookings { get; set; } // Initialize to avoid warnings
        // Add any additional properties you want for the user
    }
}
