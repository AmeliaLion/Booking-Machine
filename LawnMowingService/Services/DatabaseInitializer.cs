using LawnMowingService.Data;
using LawnMowingService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LawnMowingService.Services
{
    public class DatabaseInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                await InitializeDatabase(services);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task InitializeDatabase(IServiceProvider services)
        {
            await CreateRoles(services);
            await CreateDefaultUsers(services);
            await SeedMachines(services);
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "User", "Operator" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Role {roleName} created successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"Role {roleName} already exists.");
                }
            }
        }

        private async Task CreateDefaultUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Customer>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create admin user
            var adminEmail = serviceProvider.GetRequiredService<IConfiguration>()["AdminUser:Email"];
            var adminPassword = serviceProvider.GetRequiredService<IConfiguration>()["AdminUser:Password"];
            var adminUser = new Customer
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = serviceProvider.GetRequiredService<IConfiguration>()["AdminUser:Name"]
            };

            await CreateUserIfNotExists(userManager, roleManager, serviceProvider, adminUser, adminPassword, "Admin");

            // Create operator users
            for (int i = 1; i <= 2; i++)
            {
                var operatorEmail = serviceProvider.GetRequiredService<IConfiguration>()[($"OperatorUser{i}:Email")];
                var operatorPassword = serviceProvider.GetRequiredService<IConfiguration>()[($"OperatorUser{i}:Password")];
                var operatorName = serviceProvider.GetRequiredService<IConfiguration>()[($"OperatorUser{i}:Name")];

                var operatorUser = new Customer
                {
                    UserName = operatorEmail,
                    Email = operatorEmail,
                    Name = operatorName
                };

                await CreateUserIfNotExists(userManager, roleManager,serviceProvider, operatorUser, operatorPassword, "Operator");
            }
        }

        private async Task SeedMachines(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Machines.Any())
            {
                var machines = new List<Machine>
                {
                    new Machine { Name = "TurboMower 224", Type = "Mower", ImageUrl = "path/to/turbomower224.jpg" },
                    new Machine { Name = "PowerCutter 112", Type = "Cutter", ImageUrl = "path/to/powercutter112.jpg" }
                };
                await context.Machines.AddRangeAsync(machines);
                await context.SaveChangesAsync();
                Console.WriteLine("Machines seeded successfully.");
            }
            else
            {
                Console.WriteLine("Machines already exist in the database.");
            }
        }

        private async Task CreateUserIfNotExists(
    UserManager<Customer> userManager,
    RoleManager<IdentityRole> roleManager,
    IServiceProvider serviceProvider, // Add this parameter
    Customer user,
    string password,
    string role)
        {
            var userExist = await userManager.FindByEmailAsync(user.Email);
            if (userExist == null)
            {
                // Create the user
                var createUserResult = await userManager.CreateAsync(user, password);
                if (createUserResult.Succeeded)
                {
                    // Fetch the role based on the role name
                    var roleExist = await roleManager.FindByNameAsync(role);
                    if (roleExist != null)
                    {
                        // Directly add to aspnetuserroles
                        var userId = user.Id; // Get the created user's ID
                        var roleId = roleExist.Id; // Get the role ID

                        // Manually add the record to aspnetuserroles
                        var userRole = new IdentityUserRole<string>
                        {
                            UserId = userId,
                            RoleId = roleId
                        };

                        // Use the service provider to get the context
                        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                        context.UserRoles.Add(userRole);
                        await context.SaveChangesAsync();

                        Console.WriteLine($"User {user.UserName} created and assigned to role {role}.");
                    }
                    else
                    {
                        Console.WriteLine($"Role {role} does not exist.");
                    }
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        Console.WriteLine($"Error creating user {user.UserName}: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"User {user.Email} already exists.");
            }
        }


    }
}
