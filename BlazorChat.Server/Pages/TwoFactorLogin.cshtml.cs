using System.Threading.Tasks;
using BlazorChat.Server.Data;
using BlazorChat.Server.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorChat.Server.Pages
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
        
        public void OnGet(string errorMessage) 
        {
            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }
        public async Task<ActionResult> OnPostAsync()
        {
            string formattedAuthenticatorCode = authenticatorCode.Replace(" ", string.Empty);
            var signInResult = await signInManager.TwoFactorAuthenticatorSignInAsync(formattedAuthenticatorCode, true, false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
        }
    }
}
