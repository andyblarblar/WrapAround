using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WrapAround.LevelEditor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient()
                .AddFileReaderService(opt => opt.UseWasmSharedBuffer = true);

            await builder.Build().RunAsync();
        }
    }
}
