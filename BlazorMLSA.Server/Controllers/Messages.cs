using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Data.Chat;
using BlazorMLSA.Server.Data.Identity;
using BlazorMLSA.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ChatContext chatContext;
        private UserManager<ApplicationUser> userManager;
        public MessagesController(UserManager<ApplicationUser> userManager, ChatContext chatContext)
        {
            this.chatContext = chatContext;
            this.userManager = userManager;
        }
        public async Task<IEnumerable<MessageDto>> Get()
        {
            var id = new Guid(userManager.GetUserId(User));
            return chatContext.Messages
                .Where(message => message.SenderId == id || message.ReceiverId == id)
                .Select(message => new MessageDto
                {
                    Content = message.Text,
                    ReceiverId = message.ReceiverId.ToString(),
                    SenderId = message.SenderId.ToString()
                });
        }
    }
}
