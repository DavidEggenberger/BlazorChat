using Microsoft.AspNetCore.SignalR;

namespace BlazorMLSA.Server.Utilities.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("sub")?.Value;
        }
    }
}
