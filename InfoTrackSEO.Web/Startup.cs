using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoTrackSEO.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfoTrackSEO.Web
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
            services.AddRazorPages();
            services.AddControllers();

            //Injecting the services which will be resolved using a service provider in the service factory
            services.AddScoped<SearchServiceFactory>();
            services.AddScoped<GoogleSearchService>()
                        .AddScoped<ISearchService, GoogleSearchService>(s => s.GetService<GoogleSearchService>());
            services.AddScoped<BingSearchService>()
                        .AddScoped<ISearchService, BingSearchService>(s => s.GetService<BingSearchService>());

            //Injecting http clients for each search engine
            services.AddHttpClient<GoogleSearchService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["GoogleBaseUrl"]);
            });
            services.AddHttpClient<BingSearchService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["BingBaseUrl"]);
            });

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
                app.UseExceptionHandler("/error"); //Exceptions controller - all exceptions will be routed through this controller.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

            });
        }
    }
}
