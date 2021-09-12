using BlazorMLSA.Server.Data.Chat;
using BlazorMLSA.Server.Data.Identity;
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
        private ChatContext chatContext;
        private IdentityDbContext identityDbContext;
        public OnlineUsersController(ChatContext chatContext, IdentityDbContext identityDbContext)
        {
            this.chatContext = chatContext;
            this.identityDbContext = identityDbContext;
        }
        public IEnumerable<UserDto> Get()
        {
            List<ApplicationUser> applicationUsers = identityDbContext.Users.ToList();
            var t = chatContext.Users
                .Where(user => user.IsOnline)
                .ToList()
                .Select(user => 
                {
                    ApplicationUser applicationUser = applicationUsers.Find(appUser => new Guid(appUser.Id) == user.Id);
                    return new UserDto
                    {
                        Id = user.Id.ToString(),
                        IDP = identityDbContext.UserLogins.Where(userLogin => userLogin.UserId == user.Id.ToString()).First().LoginProvider,
                        Image = applicationUser.PictureUri,
                        Name = applicationUser.UserName
                    };
                });
            return t;
        }
    }
}
