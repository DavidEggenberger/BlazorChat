using BlazorMLSA.Server.Data;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Utilities.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> ClaimsFactory;
        private readonly UserManager<ApplicationUser> UserManager;
        public ProfileService(IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, UserManager<ApplicationUser> userManager)
        {
            ClaimsFactory = claimsFactory;
            UserManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await UserManager.FindByIdAsync(sub);

            var principal = await ClaimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();

            if (context.RequestedResources.ParsedScopes.Any(s => s.ParsedName.Contains("picture")))
            {
                claims.Add(new Claim("picture", user.PictureUri));
            }
            context.IssuedClaims = claims;
        }
        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(true);
        }
    }
}
