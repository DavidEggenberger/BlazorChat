using BlazorChat.Server.Data;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ApplicationDbContext applicationDbContext;
        public MessagesController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        public async Task<IEnumerable<MessageDto>> Get()
        {
            var id = User.Claims.Where(claim => claim.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            return applicationDbContext.Messages
                .Where(message => message.SenderId == id || message.ReceiverId == id)
                .Select(message => new MessageDto
                {
                    Content = message.Text,
                    ReceiverId = message.ReceiverId,
                    SenderId = message.SenderId
                });
        }
    }
}
