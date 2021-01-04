using DanFestaJuninaCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DanFestaJuninaCore
{
    /* Settings priority
     * 4. Files (appsettings.json, appsettings.{Environment}.json
     * 3. User Secrets
     * 2. Environment Variables (Properties - launchSettings.json)
     * 1. Command-line arguments
     * 
     * _config["MyKey"] 
     */
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            // This allows the request to be handled as XML if requested by caller in the header
            // Accept: application/xml    <<<< ===============

            services.AddHttpClient();
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
            // services.AddSingleton<IDishRepository, MockDishRepository>();
            services.AddSingleton<IDishRepository, GolangDishRepository>();
            services.AddSingleton<IKrakenRepository, KrakenRepository>();

            // 22-Sep-2019 
            // Configuring the appsetting to read variables from
            //
            services.Configure<DanAppSettings>(_config.GetSection("AppSettings"));

            // AddSingleton - same instance for all requests; once per application
            // AddTransient - each time it is requested
            // AddScoped - one per request

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {

                //await context.Response.WriteAsync(
                //    "Page not found! Process Name = " + System.Diagnostics.Process.GetCurrentProcess().ProcessName +
                //    "; Environment Name: " + env.EnvironmentName + "; MyKey = " +
                //    _config["MyKey"]);

                await context.Response.WriteAsync("Page not found!");
            });
        }
    }
}
