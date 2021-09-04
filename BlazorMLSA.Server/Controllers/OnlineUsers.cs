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
    public class OnlineUsersController : ControllerBase
    {
        private List<UserDto> userDtos;
        public OnlineUsersController(List<UserDto> userDtos)
        {
            this.userDtos = userDtos;
        }
        public List<UserDto> Get()
        {
            return userDtos;
        }
    }
}
