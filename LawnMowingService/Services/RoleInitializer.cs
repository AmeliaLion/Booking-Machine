using LawnMowingService.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LawnMowingService.Services
{
    public class RoleInitializer : IRoleInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleInitializer(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task InitializeRoles()
        {
            // Define roles
            string[] roleNames = { "Admin", "User", "Operator" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the role
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
