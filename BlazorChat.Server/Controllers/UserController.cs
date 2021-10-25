using BlazorChat.Server.Data;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BlazorChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineUsersController : ControllerBase
    {
        private ApplicationDbContext applicationDbContext;
        public OnlineUsersController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        public IEnumerable<UserDto> Get()
        {
            return applicationDbContext.Users
                .Where(user => user.IsOnline)
                .ToList()
                .Select(appUser => 
                {
                    return new UserDto
                    {
                        Id = appUser.Id.ToString(),
                        IDP = applicationDbContext.UserLogins.Where(userLogin => userLogin.UserId == appUser.Id).First().LoginProvider,
                        Image = appUser.PictureUri,
                        Name = appUser.UserName
                    };
                });
        }
    }
}
