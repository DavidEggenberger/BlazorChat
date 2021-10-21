using System.Threading.Tasks;
using BlazorMLSA.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorMLSA.Server.Pages
{
    [AllowAnonymous]
    public class RecoveryCodeLoginModel : PageModel
    {
        private SignInManager<ApplicationUser> signInManager;
        public RecoveryCodeLoginModel(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        [BindProperty]
        public string RecoveryCode { get; set; }

        public RecoveryCodeLoginModel(string errorMessage)
        {
            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }
        public async Task<ActionResult> OnPost()
        {
            var signInResult = await signInManager.TwoFactorRecoveryCodeSignInAsync(RecoveryCode);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }
            return Redirect("/");
        }
    }
}
