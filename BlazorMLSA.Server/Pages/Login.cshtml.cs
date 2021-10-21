using BlazorChat.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorChat.Server.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public readonly SignInManager<ApplicationUser> SignInManager;

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            SignInManager = signInManager;
        }
        public ActionResult OnPost([FromForm] string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
    }
}
