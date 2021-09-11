using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Data.Chat;
using BlazorMLSA.Server.Data.Identity;
using BlazorMLSA.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private ChatContext chatContext;
        private UserManager<ApplicationUser> userManager;
        public ChatHub(UserManager<ApplicationUser> userManager, ChatContext chatContext)
        {
            this.userManager = userManager;
            this.chatContext = chatContext;
        }
        public async Task NewOnlineUser()
        {
            ApplicationUser appUser = await userManager.GetUserAsync(Context.User);
            if(chatContext.Users.Any(user => user.Id == new Guid(appUser.Id)) is false)
            {
                chatContext.Users.Add(new User
                {
                    Id = new Guid(appUser.Id),
                    IsOnline = true,
                    TabsOpen = 1
                });
                await chatContext.SaveChangesAsync();
                await Clients.All.SendAsync("Update");
                return;
            }
            User user = chatContext.Users.Find(new Guid(appUser.Id));
            if(user.IsOnline is false)
            {
                user.IsOnline = true;
                user.TabsOpen = 1;
                await Clients.All.SendAsync("Update");
                await chatContext.SaveChangesAsync();
                return;
            }
            if (user.IsOnline)
            {
                user.TabsOpen++;
            }
            await chatContext.SaveChangesAsync();
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            ApplicationUser appUser = await userManager.GetUserAsync(Context.User);
            User user;
            if ((user = chatContext.Users.Find(new Guid(appUser.Id))) != null)
            {
                if(user.TabsOpen > 0)
                {
                    user.TabsOpen--;
                }
                if(user.TabsOpen == 0)
                {
                    user.IsOnline = false;
                }
                await chatContext.SaveChangesAsync();
                await Clients.All.SendAsync("Update");
            }
        }
        public async Task Chat(MessageDto message)
        {
            chatContext.Messages.Add(new Message
            {
                Text = message.Content,
                ReceiverId = new Guid(message.ReceiverId),
                SenderId = new Guid(message.SenderId)
            });
            await Clients.User(message.ReceiverId).SendAsync("ReceiveMessage", message);
        }
    }
}
