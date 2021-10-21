using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorMLSA.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

            if (builder.HostEnvironment.IsDevelopment())
            {
                builder.Services.AddOidcAuthentication(options =>
                {
                    options.ProviderOptions.Authority = "https://localhost:44372";
                    options.ProviderOptions.ClientId = "BlazorClient";
                    options.ProviderOptions.ResponseType = "code";
                    options.ProviderOptions.DefaultScopes.Add("API");
                    options.AuthenticationPaths.LogOutCallbackPath = "/";
                    options.AuthenticationPaths.LogOutPath = "https://localhost:44372/logout";
                    options.AuthenticationPaths.RemoteProfilePath = "https://localhost:44372/profile";
                    options.AuthenticationPaths.RemoteRegisterPath = "https://localhost:44372/login";
                });
            }
            if(builder.HostEnvironment.IsProduction() || builder.HostEnvironment.IsStaging())
            {
                builder.Services.AddOidcAuthentication(options =>
                {
                    options.ProviderOptions.Authority = "https://localhost:44372";
                    options.ProviderOptions.ClientId = "BlazorClient";
                    options.ProviderOptions.ResponseType = "code";
                    options.ProviderOptions.DefaultScopes.Add("API");
                    options.AuthenticationPaths.LogOutCallbackPath = "/";
                    options.AuthenticationPaths.LogOutPath = "https://localhost:44372/logout";
                    options.AuthenticationPaths.RemoteProfilePath = "https://localhost:44372/profile";
                    options.AuthenticationPaths.RemoteRegisterPath = "https://localhost:44372/login";
                });
            }

            await builder.Build().RunAsync();
        }
    }
}
