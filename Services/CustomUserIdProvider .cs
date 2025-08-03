
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var httpContext = connection.GetHttpContext();
        var userId = httpContext?.Session?.GetInt32("CurrentUserId");
        return userId?.ToString();
    }
}
