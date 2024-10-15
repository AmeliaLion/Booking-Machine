using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawnMowingService.Models; // Update this with the actual namespace of your models
using LawnMowingService.Data; // Ensure this matches your project structure
using System.Linq;
using System.Threading.Tasks;

namespace LawnMowingService.Controllers
{
    public class ConflictManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConflictManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ConflictManagementDashboard
        public IActionResult ConflictManagementDashboard()
        {
            var bookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Machine)
                .Include(b => b.Operator)
                .ToList(); // Get all bookings

            ViewBag.Operators = _context.Operators.ToList();
            return View(bookings);
        }

        // POST: AssignOperator
        [HttpPost]
        public async Task<IActionResult> AssignOperator(int bookingId, int operatorId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.OperatorId = operatorId; // Update OperatorId
                booking.Status = "Assigned"; // Update the status to Assigned
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ConflictManagementDashboard");
        }

        public async Task<IActionResult> ManageConflicts(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Machine)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Find an alternative machine
            var availableMachines = await _context.Machines
                .Where(m => !_context.Bookings.Any(b => b.MachineId == m.Id && b.Date == booking.Date))
                .ToListAsync();

            ViewBag.AvailableMachines = availableMachines;
            return View(booking);
        }
    }
}
