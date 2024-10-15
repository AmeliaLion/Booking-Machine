using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawnMowingService.Models;
using LawnMowingService.Data;
using Microsoft.AspNetCore.Authorization;

namespace LawnMowingService.Controllers
{
    //[Authorize(Policy = "RequireOperatorRole")]
    public class OperatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var operatorId = GetCurrentOperatorId(); // Get the current operator's ID
            var operatorMachineId = _context.Operators
                .Where(o => o.Id == operatorId)
                .Select(o => o.MachineId)
                .FirstOrDefault();

            var bookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Machine)
                .Where(b => b.MachineId == operatorMachineId) // Filter by the operator's machine ID
                .ToList();

            return View(bookings);
        }

        private int GetCurrentOperatorId()
        {
            // Implement logic to retrieve the current operator's ID
            // This could be from the User claims or session, depending on your authentication setup
            // For example:
            // return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return 1; // Placeholder for demonstration; replace with actual logic
        }

        public IActionResult OperatorDashboard()
        {
            return View();
        }
    }
}
