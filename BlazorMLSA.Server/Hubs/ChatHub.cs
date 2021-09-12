using BlazorMLSA.Server.Data;
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
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext applicationDbContext;
        public ChatHub(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }
        public async Task NewOnlineUser()
        {
            ApplicationUser appUser = await userManager.GetUserAsync(Context.User);
            if(appUser.IsOnline is false)
            {
                appUser.IsOnline = true;
                appUser.TabsOpen = 1;
                await applicationDbContext.SaveChangesAsync();
                await Clients.All.SendAsync("NewUser", new UserDto
                {
                    Id = appUser.Id,
                    IDP = applicationDbContext.UserLogins.Where(userLogin => userLogin.UserId == appUser.Id.ToString()).First().LoginProvider,
                    Image = appUser.PictureUri,
                    Name = appUser.UserName
                });
                return;
            }
            if (appUser.IsOnline)
            {
                appUser.TabsOpen++;
                await applicationDbContext.SaveChangesAsync();
            }
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            ApplicationUser appUser = await userManager.GetUserAsync(Context.User);
            if(appUser.TabsOpen > 0)
            {
                appUser.TabsOpen--;
            }
            if(appUser.TabsOpen == 0)
            {
                appUser.IsOnline = false;
                await applicationDbContext.SaveChangesAsync();
                await Clients.All.SendAsync("RemoveUser", new UserDto
                {
                    Id = appUser.Id,
                    IDP = applicationDbContext.UserLogins.Where(userLogin => userLogin.UserId == appUser.Id.ToString()).First().LoginProvider,
                    Image = appUser.PictureUri,
                    Name = appUser.UserName
                });
            }
        }
        public async Task Chat(MessageDto message)
        {
            applicationDbContext.Messages.Add(new Message
            {
                Text = message.Content,
                ReceiverId = message.ReceiverId,
                SenderId = message.SenderId
            });
            await applicationDbContext.SaveChangesAsync();
            await Clients.User(message.ReceiverId).SendAsync("ReceiveMessage");
        }
    }
}
