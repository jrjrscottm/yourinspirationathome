using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Hydrogen.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Infrastructure
{
    public static class UseGlobalExceptionsExtensions
    {
        public static IApplicationBuilder UseGlobalExceptions(this IApplicationBuilder app)
        {
            var signInManager = app.ApplicationServices.GetRequiredService<SignInManager<IdentityUser>>();
            var log = app.ApplicationServices.GetRequiredService<ILogger>();

            app.Use(next => new GlobalExceptionMiddleware(next, signInManager, log).Invoke);
            return app;
        }
    }

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _log;
        private readonly SignInManager<IdentityUser> _signInManager;

        public GlobalExceptionMiddleware(RequestDelegate next, SignInManager<IdentityUser> signInManager, ILogger log)
        {
            _next = next;
            _signInManager = signInManager;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnknownUserException e)
            {
                _log.Warning("Unknown user detected. Redirecting to login.");
                context.Response.Redirect("/login");
            }
            catch (Exception e)
            {
                _log.Fatal(e, "An unknown error occurred");
            }
        }
    }
}
