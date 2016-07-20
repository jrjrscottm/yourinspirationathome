using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Hydrogen.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;
using Hydrogen.Infrastructure.Authorization;
using Hydrogen.Core.Domain.Multitenancy;
using Hydrogen.Infrastructure.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Hydrogen.Data;

using Hydrogen.Models;
using Braintree;
using Hydrogen.Services.Payments;
using Hydrogen.Infrastructure;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Threading.Tasks;
using Hydrogen.Infrastructure.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("environment", env.EnvironmentName)
                .MinimumLevel.Is(
                (LogEventLevel)Enum.Parse(typeof(LogEventLevel), Configuration["Logging:LogLevel:Default"]))
                .WriteTo.ColoredConsole()
                .CreateLogger();

        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMultitenancy<ApplicationTenant, ApplicationTenantResolver>();
            // Add framework services.
            services.AddDbContext<HydrogenApplicationContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<HydrogenApplicationContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IClaimsTransformer, HydrogenClaimsTransformer>();
            services.Replace(ServiceDescriptor.Scoped<IUserClaimsPrincipalFactory<IdentityUser>, ApplicationClaimsPrincipalFactory>());


            services.AddMemoryCache();
            services.AddSession();

            //services.Configure<MvcOptions>(opt =>
            //{
            //    opt.ModelBinderProviders..Add(new HydrogenModelBinderProvider());
            //});

            services.AddMvc();

            services.AddHydrogenServices(Configuration);

            if (bool.Parse(Configuration["Logging:EnableGlimpse"]))
            {
                //services.AddGlimpse();
            }

            // Add application services.
            services.AddTransient<IApplicationTenantResolver, ApplicationTenantResolver>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();


            services.Configure<MultitenancyOptions>(options =>
            {
                var section = Configuration.GetSection("Multitenancy");
                section.Bind(options);
                //TODO: Figure out where "Proton" is coming from.
                options.DefaultTenant = "Agel";
            });

            services.Configure<RazorViewEngineOptions>(opt =>
            {
                opt.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });

            services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton(Log.Logger);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            loggerFactory.AddSerilog();

            if (env.IsDevelopment() || env.IsEnvironment("Azure"))
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseStatusCodePages();
                app.UseStatusCodePagesWithRedirects("/error/{0}");
                loggerFactory.AddDebug(LogLevel.Debug);

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<HydrogenApplicationContext>()
                             .Database.Migrate();
                    }
                }
                catch(Exception e)
                {
                    loggerFactory.CreateLogger("Startup").LogWarning("Error migrating database. {message}", e.Message);
                }
            }

            if (bool.Parse(Configuration["Logging:EnableGlimpse"]))
            {
                //app.UseGlimpse();
            }

            //app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());
            //app.UseGlobalExceptions();

            app.UseMultitenancy<ApplicationTenant>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "ExternalTemp",
                AutomaticAuthenticate = false,
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/account/login"),
                AccessDeniedPath = new PathString("/account/forbidden"),
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
            });

            app.UsePerTenant<ApplicationTenant>((context, builder) =>
            {
                if (context.Tenant.SupportedLoginTypes != null
                        && context.Tenant.SupportedLoginTypes.Contains("google"))
                {

                    var clientId = Configuration[$"{context.Tenant.Id}:GoogleClientId"];
                    var clientSecret = Configuration[$"{context.Tenant.Id}:GoogleClientSecret"];

                    if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                    {
                        Log.Warning("Using {clientId}, {clientSecret} for tenant {tenant}", clientId, clientSecret, context.Tenant.Name);


                        builder.UseGoogleAuthentication(new GoogleOptions
                        {
                            AuthenticationScheme = "Google",
                            SignInScheme = "ExternalTemp",

                            //CallbackPath = "/account/externallogincallback",

                            ClientId = Configuration[$"{context.Tenant.Id}:GoogleClientId"],
                            ClientSecret = Configuration[$"{context.Tenant.Id}:GoogleClientSecret"],
                            Events = new OAuthEvents
                            {
                                OnTicketReceived = c =>
                                {
                                    
                                    return Task.FromResult(0);
                                },
                                OnCreatingTicket = c =>
                                {
                                    return Task.FromResult(0);
                                }
                            }

                        });
                    } else
                    {
                        Log.Warning("No Google client secret found, skipping for tenant @{tenant}", context.Tenant.Name);
                    }
                }
            });

            app.UseStaticFiles();
            app.UseSession();

            app.UseIdentity();

            app.UseClaimsTransformation(new ClaimsTransformationOptions
            {
                Transformer = new HydrogenClaimsTransformer()
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
