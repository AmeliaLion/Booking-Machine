using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawnMowingService.Models;
using LawnMowingService.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using LawnMowingService.ViewModels; // Add this if it's not already present
using Microsoft.AspNetCore.Authorization; // Ensure this is included

namespace LawnMowingService.Controllers
{
    [Authorize] // Require authentication for all actions in this controller
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value; // or any other unique identifier
            var customerId = _context.Users.SingleOrDefault(u => u.Email == userEmail)?.Id;
            ViewBag.Machines = _context.Machines.ToList();
            ViewBag.CustomerId = customerId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking _booking)
        {
            if (ModelState.IsValid)
            {
                var isAvailable = !await _context.Bookings.AnyAsync(b =>
                    b.MachineId == _booking.MachineId &&
                    b.Date == _booking.Date);

                if (!isAvailable)
                {
                    ModelState.AddModelError("", "The selected machine is not available on this date.");
                    ViewBag.Machines = _context.Machines.ToList();
                    return View(_booking);
                }

                // Retrieve the user ID from the session
                var customerId = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(customerId))
                {
                    return RedirectToAction("Login", "Account"); // Redirect to login if user ID is not found
                }

                var booking = new Booking
                {
                    CustomerId = customerId, // Use the user ID from the session
                    Date = _booking.Date,
                    Status = _booking.Status,
                    MachineId = _booking.MachineId,
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction("History");
            }

            ViewBag.Machines = _context.Machines.ToList();
            return View(_booking);
        }

        public async Task<IActionResult> Confirmation(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Operator) // Ensure the Operator is included
                .Include(b => b.Machine) // Include Machine if needed
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            return RedirectToAction("History"); // Redirect to the history page after confirmation
        }

        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> History()
        {
            var customerId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            // Retrieve bookings for the logged-in user
            var bookings = await _context.Bookings
                .Where(b => b.CustomerId == customerId) // Filter by customer ID
                .Include(b => b.Machine) // Include related Machine data
                .ToListAsync();

            // Pass the bookings directly to the view
            return View(bookings); // Pass bookings of type List<Booking>
        }

        public async Task<IActionResult> ManageConflicts(int bookingId)
        {
            // Find the conflicting booking
            var booking = await _context.Bookings
                .Include(b => b.Machine)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Get a list of available machines for the same date
            var availableMachines = await _context.Machines
                .Where(m => !_context.Bookings.Any(b => b.MachineId == m.Id && b.Date == booking.Date))
                .ToListAsync();

            ViewBag.AvailableMachines = availableMachines;
            return View(booking);
        }

    }
}
