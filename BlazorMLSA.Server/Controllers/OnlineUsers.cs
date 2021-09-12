using BlazorMLSA.Server.Data;
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
        private IdentityDbContext identityDbContext;
        public OnlineUsersController(IdentityDbContext identityDbContext)
        {
            this.identityDbContext = identityDbContext;
        }
        public IEnumerable<UserDto> Get()
        {
            List<ApplicationUser> applicationUsers = identityDbContext.Users.ToList();
            var t = identityDbContext.Users
                .Where(user => user.IsOnline)
                .ToList()
                .Select(user => 
                {
                    ApplicationUser applicationUser = applicationUsers.Find(appUser => appUser.Id == user.Id);
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
