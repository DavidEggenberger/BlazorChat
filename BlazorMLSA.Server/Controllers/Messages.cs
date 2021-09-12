using BlazorMLSA.Server.Data;
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
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext applicationDbContext;
        public MessagesController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }
        public async Task<IEnumerable<MessageDto>> Get()
        {
            var id = userManager.GetUserId(User);
            return applicationDbContext.Messages
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
