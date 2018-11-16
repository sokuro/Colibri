using AutoMapper;
using System.Text;
using Colibri.Data;
using Colibri.Middleware;
using Colibri.Models;
using Colibri.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Colibri
{
    public class Startup
    {
        // private Fields
        private readonly IConfiguration _configuration;

        // CTOR that is injectable
        // define the SqlServer connection string
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Services will be used by dependency injection
        public void ConfigureServices(IServiceCollection services)
        {
            // add Identity Service
            //services.AddIdentity<ApplicationUser, IdentityRole>(cfg =>
            //{
            //    cfg.User.RequireUniqueEmail = true;
            //})
            //// to separate Contextes
            //.AddEntityFrameworkStores<ColibriDbContext>();

            // Add Authentication
            // -> Cookies
            // -> Token
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    // validation Parameters needed
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = _configuration["Tokens:Issuer"],
                        ValidAudience = _configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]))
                    };
                });

            // Adding ICategoryData Service
            services.AddScoped<ICategoryTypesData, SqlCategoryTypesData>();

            // Adding DbContext Service
            services.AddDbContext<ColibriDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("Colibri")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ColibriDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            // Support for AutoMapper
            services.AddAutoMapper();

            // Adding IMailService
            services.AddTransient<IMailService, NullMailService>();

            // Adding Email Service using Core 2.1 IEmailSender Interface
            services.AddTransient<IEmailSender, EmailSender>();

            // register the AuthMessageSenderOptions Instance
            services.Configure<AuthMessageSenderOptions>(_configuration);

            // fill the DB with Entries
            //services.AddTransient<ColibriSeeder>();

            // Repository Layer between the actual DB
            // #1: User can use the IColibriRepository
            // #2: but use the Implementation of the ColibriRepository
            services.AddScoped<IColibriRepository, ColibriRepository>();

            // Adding MVC Services
            services.AddMvc()
                //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddSignalR();

            // Adding Sessions
            services.AddSession(opt =>
            {
                // set Session TimeOut to t=30m
                opt.IdleTimeout = TimeSpan.FromMinutes(30);
                // use Cookies for Http
                opt.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // DEV USE Middleware
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            // (always use) SSL Middleware
            app.UseRewriter();

            // #1: Middleware: serve up Files from the 'wwwroot'
            // use Static Files to reach the Index Home Page
            app.UseStaticFiles();

            // #2: Instance to serve Files from the /node_modules
            //app.UseNodeModules(env.ContentRootPath);

            // #3.5 (later implemented) enables Identity Service
            app.UseAuthentication();

            // Start a Session
            app.UseSession();

            // Default MVC Route
            app.UseMvcWithDefaultRoute();

            // use MVC with the configured Route
            app.UseMvc(ConfigureRoutes);
        }

        // Configure Routes: using IRouteBuilder Routing Interface
        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            // Routing with Areas
            // "~/area/Home/Index"
            routeBuilder.MapRoute(
                name: "area",
                template: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
                );
            // Default Route: "~/Home/Index"
            routeBuilder.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}"
                );
        }
    }
}
