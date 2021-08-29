using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazorMLSA.Server.EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace BlazorMLSA.Server.Pages
{
    public class ProfileModel : PageModel
    {
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }
        private readonly UrlEncoder _urlEncoder;
        public ProfileModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _urlEncoder = urlEncoder;
        }
        public bool TwoFAEnabled { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [BindProperty]
        public string AuthenticatorCode { get; set; }
        public string Base64 { get; set; }
        public async Task OnGet()
        {
            ApplicationUser = await UserManager.GetUserAsync(User);
            TwoFAEnabled = await UserManager.GetTwoFactorEnabledAsync(ApplicationUser);

            var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(ApplicationUser);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await UserManager.ResetAuthenticatorKeyAsync(ApplicationUser);
                unformattedKey = await UserManager.GetAuthenticatorKeyAsync(ApplicationUser);
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
            ApplicationUser = await UserManager.GetUserAsync(User);

            var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(
                ApplicationUser, UserManager.Options.Tokens.AuthenticatorTokenProvider, AuthenticatorCode);

            if (is2faTokenValid)
            {
                await UserManager.SetTwoFactorEnabledAsync(ApplicationUser, true);
            }

            return Redirect("/");
        }
        private string GenerateQrCodeUri(string unformattedKey)
        {
            const string AuthenticatorUriFormat = "otpauth://totp/{0}?secret={1}&digits=6";

            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Let's Chat!"),
                unformattedKey);
        }
    }
}
