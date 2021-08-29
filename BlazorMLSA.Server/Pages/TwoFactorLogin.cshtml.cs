using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMLSA.Server.EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorMLSA.Server.Pages
{
    public class TwoFactorLoginModel : PageModel
    {
        [BindProperty]
        public string returnUrl { get; set; }
        [BindProperty]
        public string authenticatorCode { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }
        public TwoFactorLoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public void OnGet()
        {
        }
        public async Task<ActionResult> OnPostAsync()
        {
            ApplicationUser user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            var resutl = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, true, false);
            return Redirect("/");
        }
    }
}
