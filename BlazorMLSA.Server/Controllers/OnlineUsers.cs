using BlazorMLSA.Shared;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = IdentityServerJwtConstants.IdentityServerJwtBearerScheme)]
    public class OnlineUsers : ControllerBase
    {
        public List<UserDto> Get([FromServices] List<UserDto> userDtos)
        {
            var r = HttpContext.Request;
            var user = User;
            return userDtos;
        }
    }
}
