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
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;

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
            // Add Authentication
            // -> Cookies
            // -> Token
            services.AddAuthentication()
                .AddCookie();
            //.AddJwtBearer(cfg =>
            //{
            //    // validation Parameters needed
            //    cfg.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidIssuer = _configuration["Tokens:Issuer"],
            //        ValidAudience = _configuration["Tokens:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]))
            //    };
            //});
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies 
                // is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add Authentication
            // -> Facebook
            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = "2160036487581365";
                    facebookOptions.AppSecret = "32b1d1a4a8119468a680654eb6b6db45";
                    // Azure
                    //facebookOptions.AppId = "593636237757896";
                    //facebookOptions.AppSecret = "2b1f39841810aa12a8ac583ccadc1b10";
                });

            // Add Authentication
            // -> Google+
            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "361598547615-m4hpnviha4v4j1jm33t6gar197lss7jq.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "q_6PYvO3wjkLa44fEzufpqhd";
                    // Azure
                    //googleOptions.ClientId = "831326771786-mhe9g88ou0vlenrsgfvphvh8q7envc2g.apps.googleusercontent.com";
                    //googleOptions.ClientSecret = "221EXPkc9ODmojPvJyQhA1oi";
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

            // Adding Email Service using Core 2.1 IEmailSender Interface
            services.AddTransient<IEmailSender, EmailSender>();

            // register the AuthMessageSenderOptions Instance
            services.Configure<AuthMessageSenderOptions>(_configuration);

            // fill the DB with Entries
            //services.AddTransient<ColibriSeeder>();

            // i18n
            //services.AddApplicationInsightsTelemetry(_configuration);
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("de-DE")
                };

                //options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.DefaultRequestCulture = new RequestCulture(culture: "de-DE", uiCulture: "de-DE");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            // Adding MVC Services
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddRazorPagesOptions(options => options.AllowAreas = true)
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
            // i18n
            var localizationOption = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOption.Value);

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

            app.UseCookiePolicy();

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
