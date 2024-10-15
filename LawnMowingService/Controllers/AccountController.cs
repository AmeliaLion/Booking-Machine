using LawnMowingService.Data;
using LawnMowingService.Models;
using LawnMowingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Make sure to include this namespace for EF
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LawnMowingService.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly ApplicationDbContext _context; // Add ApplicationDbContext to access the database
        private readonly IServiceProvider serviceProvider; // For ApplicationDbContext
        private readonly UserManager<Customer> userManager; // Inject UserManager
        private readonly RoleManager<IdentityRole> roleManager; // Inject RoleManager
        private readonly UserManagementService _userManagementService;


        public AccountController(UserManager<Customer> userManager, SignInManager<Customer> signInManager, ApplicationDbContext context, IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager, UserManagementService userManagementService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Initialize ApplicationDbContext
            this.serviceProvider = serviceProvider;
            this.roleManager = roleManager;
            _userManagementService = userManagementService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
        {
            var user = new Customer
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name
            };

            var role = "User"; // You may want to fetch or determine this dynamically

            if (await _userManagementService.CreateUserIfNotExists(user, model.Password, role))
            {
                // User created successfully
                return RedirectToAction("Login"); // Or wherever you want to redirect after successful registration
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User with this email already exists."); // Inform the user
            }
        }

        return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email ?? string.Empty, model.Password ?? string.Empty, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null) // Check if user exists
                    {
                        HttpContext.Session.SetString("UserID", user.Id.ToString());
                        // Retrieve roles using the custom method
                        var roles = await GetUserRolesAsync(user); // Use custom method

                        HttpContext.Session.SetString("UserName", user.Name);


                        // Redirect based on user role
                        if (roles.Contains("Admin"))
                        {
                            HttpContext.Session.SetString("UserRoles", "Admin");
                            return RedirectToAction("ConflictManagementDashboard", "ConflictManager"); // Redirect to admin page
                        }
                        else if (roles.Contains("Operator"))
                        {
                            HttpContext.Session.SetString("UserRoles", "Operator");
                            return RedirectToAction("Dashboard", "Operator"); // Redirect to operator dashboard
                        }
                        else
                        {
                            HttpContext.Session.SetString("UserRoles", "User");
                            return RedirectToAction("Create", "Booking"); // Default redirect for normal users
                        }
                    }
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }


        // Custom method to retrieve user roles
        private async Task<List<string>> GetUserRolesAsync(Customer user)
        {
            var userId = user.Id; // Get the user ID
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            return await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Customer");
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

    }
}
