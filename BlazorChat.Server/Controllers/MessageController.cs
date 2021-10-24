using BlazorChat.Server.Data;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ApplicationDbContext applicationDbContext;
        private UserManager<ApplicationUser> userManager;
        public MessagesController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
        }
        public async Task<IEnumerable<MessageDto>> Get()
        {
            ApplicationUser appUser = await userManager.GetUserAsync(HttpContext.User);
            return applicationDbContext.Messages
                .Where(message => message.SenderId == appUser.Id || message.ReceiverId == appUser.Id)
                .Select(message => new MessageDto
                {
                    Content = message.Text,
                    ReceiverId = message.ReceiverId,
                    SenderId = message.SenderId
                });
        }
    }
}
