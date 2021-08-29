using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMLSA.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorMLSA.Server.Pages
{
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
        }
        public ActionResult OnPost([FromForm] string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
    }
}
