using System.Threading.Tasks;
using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorMLSA.Server.Pages
{
    [AllowAnonymous]
    public class TwoFactorLoginModel : PageModel
    {
        private SignInManager<ApplicationUser> signInManager;
        public TwoFactorLoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        [BindProperty]
        public string authenticatorCode { get; set; }
        
        public void OnGet() { }
        public async Task<ActionResult> OnPostAsync()
        {
            var signInResult = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, true, false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }
            return Redirect("/");
        }
    }
}
