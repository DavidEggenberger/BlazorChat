using BlazorMLSA.Server.Data;
using BlazorMLSA.Server.Data.Chat;
using BlazorMLSA.Server.Data.Identity;
using BlazorMLSA.Server.Hubs;
using BlazorMLSA.Server.Utilities.IdentityServer;
using BlazorMLSA.Server.Utilities.LinkedInPicture;
using BlazorMLSA.Server.Utilities.SignalR;
using BlazorMLSA.Shared;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
            services.AddHttpClient("github", client =>
            {
                client.BaseAddress = new Uri("https://api.linkedin.com/v2");
            });
            services.AddControllers();
            services.AddRazorPages();
            services.AddSignalR();

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<ChatContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                options.DefaultChallengeScheme = IdentityServerJwtConstants.IdentityServerJwtBearerScheme;
                options.DefaultAuthenticateScheme = "ApplicationDefinedAuthentication";
            })
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
                    options.Events.OnCreatingTicket = context =>
                    {
                        string picUri = context.User.GetProperty("avatar_url").GetString();
                        context.Identity.AddClaim(new Claim("picture", picUri));
                        return Task.CompletedTask;
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
                })
                .AddIdentityCookies(o => { });
            var identityService = services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                o.Stores.MaxLengthForKeys = 128;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<IdentityDbContext>();
            identityService.AddSignInManager();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, IdentityDbContext>(options =>
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
                            "API"
                        },
                        RedirectUris = { "https://localhost:44372/authentication/login-callback" },
                        PostLogoutRedirectUris = { "https://localhost:44372" },
                        FrontChannelLogoutUri = "https://localhost:44372"
                    });
                    options.ApiResources = new ApiResourceCollection
                    {
                        new ApiResource
                        {
                            Name = "API",
                            Scopes = new List<string> {"API"}
                        }
                    };
                    var cert = options.SigningCredential = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("David Eggenberger Security key very long very secure")), SecurityAlgorithms.HmacSha256);
                })
                .AddProfileService<ProfileService>();

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
            services.Configure<AntiforgeryOptions>(options =>
            {
                options.Cookie.Name = "AntiforgeryCookie";
            });
            services.Configure<CookieTempDataProviderOptions>(options => { options.Cookie.Name = "TemporaryDataCookie"; });
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
                endpoints.MapHub<ChatHub>("/chathub").RequireAuthorization();
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapRazorPages().RequireAuthorization();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
