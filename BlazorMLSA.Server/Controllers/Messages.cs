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
    public class Messages : ControllerBase
    {
        public async Task<List<MessageDto>> Get([FromServices] List<MessageDto> messages, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var id = userManager.GetUserId(User);
            var v = await HttpContext.AuthenticateAsync();     
            return messages.Where(m => m.ReceiverId == id || m.SenderId == id).ToList();
        }
    }
}
