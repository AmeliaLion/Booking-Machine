using LawnMowingService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly List<string> _notifications = new List<string>();

    public async Task NotifyUser(string message)
    {
        // Simulate adding a notification
        _notifications.Add(message);
        await Task.CompletedTask; // Placeholder for actual notification logic
    }

    public List<string> GetNotifications()
    {
        return _notifications;
    }
}
