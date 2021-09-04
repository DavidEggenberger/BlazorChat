using BlazorMLSA.Server.Data;
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
        List<UserDto> OnlineUsers;
        UserManager<ApplicationUser> UserManager;
        List<MessageDto> MessageDtos;
        public ChatHub(List<UserDto> onlineUsers, List<MessageDto> messageDtos, UserManager<ApplicationUser> userManager)
        {
            OnlineUsers = onlineUsers;
            UserManager = userManager;
            MessageDtos = messageDtos;
        }
        public async Task SendMessage(string UserName, string message)
        {
            await Clients.User(UserName).SendAsync("ReceiveMessage", message);
        }
        public async Task NewOnlineUser()
        {
            UserDto userDto = new UserDto { Name = Context.User.Identity.Name, IDP = Context.User.Claims.Where(c => c.Type == "idp").First().Value };
            ApplicationUser appUser = await UserManager.GetUserAsync(Context.User);
            userDto.Image = appUser.PictureUri;
            userDto.Id = appUser.Id;
            if(OnlineUsers.Where(user => user.Name == userDto.Name).Count() == 0)
            {
                OnlineUsers.Add(userDto);
            }

            await Clients.All.SendAsync("Update");
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            UserDto userDto = new UserDto { Name = Context.User.Identity.Name };
            if (OnlineUsers.Where(user => user.Name == userDto.Name).Count() == 1)
            {
                OnlineUsers.Remove(OnlineUsers.Where(s => s.Name == userDto.Name).First());
            }         
            await Clients.All.SendAsync("Update", userDto);
        }
        public async Task Chat(MessageDto message)
        {
            MessageDtos.Add(message);
            await Clients.User(message.ReceiverId).SendAsync("ReceiveMessage", message);
        }
    }
}
