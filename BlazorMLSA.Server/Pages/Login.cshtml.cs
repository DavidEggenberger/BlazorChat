using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMLSA.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorMLSA.Server.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private SignInManager<ApplicationUser> _signInManager;
        public string returnUrl { get; set; }
        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public void OnGet(string returnUrl)
        {
            this.returnUrl = returnUrl;
            var u = HttpContext.User;
        }
        public ActionResult OnPost([FromForm] string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string Email { get; set; }
        public async Task<ActionResult> OnPostLocal([FromServices] UserManager<ApplicationUser> us, string returnUrl = null)
        {
            if(us.Users.Count() < 300)
            {
                await us.CreateAsync(new ApplicationUser
                {
                    UserName = "David",
                    Email = "david.eggenberger@hotmail.com",
                    EmailConfirmed = true
                }, "Password@123");
            }
            ApplicationUser user = await us.FindByEmailAsync(Email);
            var rest = await _signInManager.PasswordSignInAsync(user, Password, true, true);
            return LocalRedirect(returnUrl);
        }
    }
}
