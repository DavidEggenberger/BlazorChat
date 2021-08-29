using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Hubs;
using BlazorMLSA.Server.Utilities.IdentityServer;
using BlazorMLSA.Server.Utilities.LinkedInPicture;
using BlazorMLSA.Server.Utilities.SignalR;
using BlazorMLSA.Shared;
using IdentityModel.Client;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorMLSA.Server
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();
            services.AddSingleton<List<UserDto>>();
            services.AddSingleton<List<MessageDto>>();
            
            services.AddControllers();
            services.AddRazorPages();
            services.AddSignalR();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
            });

            services.Configure<CookieAuthenticationOptions>
                (IdentityConstants.ApplicationScheme, op =>
            {
                op.ExpireTimeSpan = TimeSpan.FromSeconds(10000);
            });
            services.Configure<CookieAuthenticationOptions>
            (IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = "app.2fa.rememberme";
                o.ExpireTimeSpan = TimeSpan.FromSeconds(10);
            });

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.Clients.Add(new IdentityServer4.Models.Client
                    {
                        ClientId = "BlazorClient",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        AllowedScopes = new List<string>
                        {
                            "openid",
                            "profile",
                            "picture"
                        },
                        AccessTokenLifetime = 500,
                        RedirectUris = { "https://localhost:44372/authentication/login-callback" },
                        PostLogoutRedirectUris = { "https://localhost:44372" },
                        FrontChannelLogoutUri = "https://localhost:44372"
                    });
                    options.IdentityResources.Add(new IdentityResource
                    {
                        Name = "picture",
                        DisplayName = "Picture",
                        UserClaims = new List<string> { ClaimTypes.WindowsAccountName}
                    });
                    options.SigningCredential = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("David Eggenberger Very secure and long password!!!")), SecurityAlgorithms.HmacSha256);
                })
                .AddProfileService<ProfileService>();

            services.AddAuthentication()
                .AddLinkedIn(options =>
                {
                    options.ClientId = Configuration["LinkedIn:ClientId"];
                    options.ClientSecret = Configuration["LinkedIn:ClientSecret"]; ;
                    options.Scope.Add("r_liteprofile");
                    options.Events.OnCreatingTicket = async context =>
                    {
                        HttpClient htp = new HttpClient();
                        htp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        var respone = await htp.GetStringAsync("https://api.linkedin.com/v2/me?projection=(id,profilePicture(displayImage~:playableStreams))");
                        Root root = JsonSerializer.Deserialize<Root>(respone);
                        context.Identity.AddClaim(new Claim("picture", root.profilePicture.DisplayImage.elements.Skip(1).First().identifiers.First().identifier));
                    };
                })
                .AddGitHub(options =>
                {
                    options.ClientId = Configuration["GitHub:ClientId"];
                    options.ClientSecret = Configuration["GitHub:ClientSecret"];
                    options.Events.OnCreatingTicket = async context =>
                    {
                        string picUri = context.User.GetProperty("avatar_url").GetString();
                        context.Identity.AddClaim(new Claim("picture", picUri));
                    };
                })
                .AddIdentityServerJwt();

            services.ConfigureApplicationCookie(options =>
            {
                options.LogoutPath = "/Logout";
                options.LoginPath = "/Login";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
