using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Components;
using BlazorMLSA.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorMLSA.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("BlazorASPCoreAuthIdentityServerClient.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorASPCoreAuthIdentityServerClient.ServerAPI"));

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.Authority = "https://localhost:44372";
                options.ProviderOptions.ClientId = "BlazorClient";
                options.ProviderOptions.ResponseType = "code";
                options.ProviderOptions.DefaultScopes.Add("Write");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("picture");
                options.ProviderOptions.DefaultScopes.Add("API");
                options.AuthenticationPaths.LogOutSucceededPath = "/";
                options.AuthenticationPaths.LogOutPath = "https://localhost:44372/logout";
                options.AuthenticationPaths.RemoteProfilePath = "https://localhost:44372/profile";
                options.AuthenticationPaths.RemoteRegisterPath = "https://localhost:44372/login";
            });

            await builder.Build().RunAsync();
        }
    }
}
