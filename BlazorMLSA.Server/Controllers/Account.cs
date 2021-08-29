using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Hubs;
using BlazorMLSA.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Server.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        public string returnUrl { get; set; }
        public Account(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var user = await _userManager.FindByNameAsync(info.Principal.Identity.Name);

            if (info is not null && user is null)
            {
                ApplicationUser _user = new ApplicationUser
                {
                    UserName = info.Principal.Identity.Name,
                    PictureUri = info.Principal.Claims.Where(c => c.Type == "picture").First().Value
                };
                var result = await _userManager.CreateAsync(_user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(_user, info);
                    await _signInManager.SignInAsync(_user, isPersistent: false, info.LoginProvider);
                    return LocalRedirect(returnUrl);
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            return signInResult switch
            {
                Microsoft.AspNetCore.Identity.SignInResult { Succeeded: true} => Redirect(returnUrl),
                Microsoft.AspNetCore.Identity.SignInResult { RequiresTwoFactor: true } => Redirect($"TwoFactorLogin?returnUrl={returnUrl}"),
                _ => Redirect("/")
            };
        }
    }
}
