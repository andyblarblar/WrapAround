using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WrapAround.hubs;
using WrapAround.Logic.Implimentations;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR(hubOption => { hubOption.EnableDetailedErrors = true; })
                .AddJsonProtocol(op =>
                {
                    op.PayloadSerializerOptions.Converters.Add(new Vector2Converter());
                })
                .AddMessagePackProtocol();

            services.AddSingleton<IServerLoop, ServerLoop>();//injects server loop
            services.AddTransient<IMapLoader, MapFileLoader>();//injects preferred map loader
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();

            app.UseStaticFiles(new StaticFileOptions{ ServeUnknownFileTypes = true});

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("/game");
            });
        }
    }
}
