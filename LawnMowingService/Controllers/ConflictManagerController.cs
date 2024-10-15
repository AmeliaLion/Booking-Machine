using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawnMowingService.Models; // Update this with the actual namespace of your models
using LawnMowingService.Data; // Ensure this matches your project structure
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LawnMowingService.Controllers
{
    public class ConflictManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Customer> _userManager;

        public ConflictManagerController(ApplicationDbContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ConflictManagementDashboard
        public async Task<IActionResult> ConflictManagementDashboard()
        {
            var userId = HttpContext.Session.GetString("UserID");
            var user = await _userManager.FindByEmailAsync(userId);

            // Retrieve all users with the "Operator" role
            var operators = await _userManager.GetUsersInRoleAsync("Operator");

            // Filter bookings to include only those with operators in the role or matching identifier
            var bookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Machine)
                .Include(b => b.Operator)
                .ToList(); // Get all bookings

            ViewBag.Operators = operators.ToList();
            return View(bookings);
        }


        // POST: AssignOperator
        [HttpPost]
        public async Task<IActionResult> AssignOperator(int bookingId, int operatorId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.OperatorId = operatorId;
                booking.Status = "Assigned";
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
