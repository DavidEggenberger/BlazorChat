﻿using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BlazorChat.Server.Utilities.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
