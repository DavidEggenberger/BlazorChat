using BlazorMLSA.Server.Data;
using BlazorMLSA.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private List<MessageDto> Messages;
        private UserManager<ApplicationUser> userManager;
        public MessagesController(List<MessageDto> messages, UserManager<ApplicationUser> userManager)
        {
            Messages = messages;
            this.userManager = userManager;
        }
        public async Task<List<MessageDto>> Get()
        {
            var id = userManager.GetUserId(User);
            return Messages.Where(m => m.ReceiverId == id || m.SenderId == id).ToList();
        }
    }
}
