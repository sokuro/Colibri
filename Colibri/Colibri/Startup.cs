using Colibri.Data;
using Colibri.Middleware;
using Colibri.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Colibri
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding ICategoryData Service
            services.AddScoped<ICategoryData, SqlCategoryData>();

            // Adding ITransportData Service
            services.AddScoped<ITransportData, SqlTransportData>();

            // Adding ICameraData Service
            services.AddScoped<ICameraData, SqlCameraData>();

            // Adding DbContext Service
            services.AddDbContext<ColibriDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("Colibri")));

            // Adding MVC Services
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // #1: Middleware: serve up Files from the 'wwwroot'
            // use Static Files to reach the Index Home Page
            app.UseStaticFiles();

            // #2: Instance to serve Files from the /node_modules
            app.UseNodeModules(env.ContentRootPath);

            // Default MVC Route
            app.UseMvcWithDefaultRoute();

            // use MVC with the configured Route
            app.UseMvc(ConfigureRoutes);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        // Configure Routes: using IRouteBuilder Routing Interface
        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            // Default Route: "~/Home/Index"
            routeBuilder.MapRoute(
                "Default",
                "{controller=Home}/{action=Index}/{id}"
                );
        }
    }
}
