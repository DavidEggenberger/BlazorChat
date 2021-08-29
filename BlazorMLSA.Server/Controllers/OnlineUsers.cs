using BlazorMLSA.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class OnlineUsers : ControllerBase
    {
        public List<UserDto> Get([FromServices] List<UserDto> userDtos) => userDtos;
    }
}
