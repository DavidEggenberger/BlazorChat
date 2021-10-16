using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace BlazorMLSA.Server.Pages
{
    public class ProfileModel : PageModel
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private UrlEncoder urlEncoder;
        public ProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.urlEncoder = urlEncoder;
        }
        [BindProperty]
        public string AuthenticatorCode { get; set; }
        [TempData]
        public string[] RecoveryCodes { get; set; }
        public bool TwoFAEnabled { get; set; }
        public ApplicationUser ApplicationUser { get; set; }    
        public string Base64 { get; set; }
        public async Task OnGet()
        {
            ApplicationUser = await userManager.GetUserAsync(User);
            TwoFAEnabled = await userManager.GetTwoFactorEnabledAsync(ApplicationUser);
            var unformattedKey = await userManager.GetAuthenticatorKeyAsync(ApplicationUser);
            
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(ApplicationUser);
                unformattedKey = await userManager.GetAuthenticatorKeyAsync(ApplicationUser);
            }
            var AuthenticatorURI = GenerateQrCodeUri(unformattedKey);
            MemoryStream ms = new MemoryStream();
            QRCodeGenerator cod = new QRCodeGenerator();
            QRCodeData data = cod.CreateQrCode(AuthenticatorURI, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            Bitmap bmp = code.GetGraphic(20, "#0a66c2", "#0d1117", true);
            bmp.Save(ms, ImageFormat.Png);
            Base64 = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
        }
        public async Task<ActionResult> OnPostAsync()
        {
            string formattedAuthenticatorCode = AuthenticatorCode.Replace(" ", string.Empty);
            ApplicationUser = await userManager.GetUserAsync(User);
            var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
                ApplicationUser, userManager.Options.Tokens.AuthenticatorTokenProvider, formattedAuthenticatorCode);

            if (is2faTokenValid)
            {
                await userManager.SetTwoFactorEnabledAsync(ApplicationUser, true);
            }

            if(await userManager.CountRecoveryCodesAsync(ApplicationUser) == 0)
            {
                RecoveryCodes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(ApplicationUser, 3)).ToArray();
                return RedirectToPage("/Profile");
            }

            await signInManager.SignOutAsync();
            return Redirect("/");
        }
        public async Task<ActionResult> OnPostDisable()
        {
            ApplicationUser = await userManager.GetUserAsync(User);           
            await userManager.SetTwoFactorEnabledAsync(ApplicationUser, false);
            return Redirect("/");
        }
        private string GenerateQrCodeUri(string unformattedKey)
        {
            const string AuthenticatorUriFormat = "otpauth://totp/{0}?secret={1}&digits=6";

            return string.Format(
                AuthenticatorUriFormat,
                urlEncoder.Encode("Let's Chat!"),
                unformattedKey);
        }
    }
}
