using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorStrap;
using Tewr.Blazor.FileReader;
using Blazored.LocalStorage;
using Blazored.Modal;
using BasicCAM.WASM.Application;
using Blazor.Extensions.Logging;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Reflection;

namespace BasicCAM.WASM
{
    public class Program
    {
        public static IWebAssemblyHostEnvironment Env { get; private set; }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder
                .Services
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddBootstrapCss()
                .AddFileReaderService(options => options.UseWasmSharedBuffer = true)
            .AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning);
                builder.AddBrowserConsole().AddFilter("Microsoft", LogLevel.Warning);
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            Env = builder.HostEnvironment;

            ConfigureServices(builder.Services);


            var host = builder.Build();

            ConfigureProviders(host.Services);

            await host.RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services)
        {

            services.AddBlazoredModal();
            services.AddScoped<BasicCAMMain>();
            services.AddOptions();


            services.AddScoped(sp =>
            new HttpClient
            {
                BaseAddress = new Uri(Env.BaseAddress)
            });

            services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.WriteIndented = true;
            });
        }

        public static void ConfigureProviders(IServiceProvider services)
        {
            try
            {
                var jsRuntime = services.GetService<IJSRuntime>();
                var prop = typeof(JSRuntime).GetProperty("JsonSerializerOptions", BindingFlags.NonPublic | BindingFlags.Instance);
                JsonSerializerOptions value = (JsonSerializerOptions)Convert.ChangeType(prop.GetValue(jsRuntime, null), typeof(JsonSerializerOptions));
                value.PropertyNamingPolicy = null;
                value.IgnoreNullValues = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SOME ERROR: {ex}");
            }
        }
    }
}
