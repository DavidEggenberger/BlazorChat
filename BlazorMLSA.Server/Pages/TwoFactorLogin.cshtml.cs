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
    public class TwoFactorLoginModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string returnUrl { get; set; }
        [BindProperty]
        public string authenticatorCode { get; set; }
        private SignInManager<ApplicationUser> signInManager;
        public TwoFactorLoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public void OnGet() { }
        public async Task<ActionResult> OnPostAsync()
        {
            ApplicationUser user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            var resutl = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, true, false);
            return Redirect("/");
        }
    }
}
