using LawnMowingService.Data;
using LawnMowingService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

public class UserManagementService
{
    private readonly UserManager<Customer> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;

    public UserManagementService(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, IServiceProvider serviceProvider, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _serviceProvider = serviceProvider;
        _context = context;
    }

    public async Task<bool> CreateUserIfNotExists(Customer user, string password, string role)
    {
        var userExist = await _userManager.FindByEmailAsync(user.Email);
        if (userExist != null)
        {
            return false; // User already exists
        }

        var createUserResult = await _userManager.CreateAsync(user, password);
        if (createUserResult.Succeeded)
        {
            // Assign role
            var roleExist = await _roleManager.FindByNameAsync(role);
            if (roleExist != null)
            {
                var userRole = new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = roleExist.Id
                };
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return true; // User created and role assigned
            }
        }

        return false; // User creation failed
    }
}
