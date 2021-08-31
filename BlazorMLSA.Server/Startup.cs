using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Hubs;
using BlazorMLSA.Server.Utilities.IdentityServer;
using BlazorMLSA.Server.Utilities.LinkedInPicture;
using BlazorMLSA.Server.Utilities.SignalR;
using BlazorMLSA.Shared;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

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
            services.AddHttpClient("github", client =>
            {
                client.BaseAddress = new Uri("https://api.linkedin.com/v2");
            });
            services.AddControllers();
            services.AddRazorPages();
            services.AddSignalR();
            services.Configure<ApiAuthorizationOptions>(options =>
            {
                options.ApiResources = new ApiResourceCollection
                {
                    new ApiResource
                    {
                        Name = "API",
                        Scopes = new List<string> {"API"}
                    }
                };
            });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddIdentityCookies(o => { });
            var identityService = services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.Stores.MaxLengthForKeys = 128;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            identityService.AddSignInManager();

            services.AddIdentityServer(options => 
            {
                
            })
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.Clients.Add(new IdentityServer4.Models.Client
                    {
                        ClientId = "BlazorClient",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequirePkce = true,
                        RequireClientSecret = false,
                        AllowedScopes = new List<string>
                        {
                            "openid",
                            "profile",
                            "picture",
                            "Write",
                            "API"
                        },
                        RedirectUris = { "https://localhost:44372/authentication/login-callback" },
                        PostLogoutRedirectUris = { "https://localhost:44372" },
                        FrontChannelLogoutUri = "https://localhost:44372"
                    });
                    options.IdentityResources.Add(new IdentityResource
                    {
                        Name = "picture",
                        DisplayName = "Picture",
                        UserClaims = new List<string> { "picture" }
                    });
                    options.ApiScopes.Add(new ApiScope
                    {
                        Name = "Write",
                    });
                    var cert = options.SigningCredential = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("David Eggenberger Security key very long very secure")), SecurityAlgorithms.HmacSha256);
                });

            services.AddAuthentication()
                .AddIdentityServerJwt()
                .AddLinkedIn(options =>
                 {
                     options.ClientId = Configuration["LinkedIn:ClientId"];
                     options.ClientSecret = Configuration["LinkedIn:ClientSecret"]; ;
                     options.Scope.Add("r_liteprofile");
                     options.Events.OnCreatingTicket = async context =>
                     {
                         HttpClient htp = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("github");
                         htp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                         var respone = await htp.GetStringAsync(htp.BaseAddress + "/me?projection=(id,profilePicture(displayImage~:playableStreams))");
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
                 .AddPolicyScheme("ApplicationDefinedAuthentication", null, options =>
                 {
                     options.ForwardDefaultSelector = (context) =>
                     {
                         if (context.Request.Path.StartsWithSegments(new PathString("/api"), StringComparison.OrdinalIgnoreCase))
                             return IdentityServerJwtConstants.IdentityServerJwtScheme;
                         else
                             return IdentityConstants.ApplicationScheme;
                     };
                 });
            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultChallengeScheme = IdentityServerJwtConstants.IdentityServerJwtBearerScheme;
                options.DefaultAuthenticateScheme = "ApplicationDefinedAuthentication";
            });
            services.Configure<JwtBearerOptions>(IdentityServerJwtConstants.IdentityServerJwtBearerScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = "API"
                };
            });
            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Login";
                config.LogoutPath = "/Logout";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

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
