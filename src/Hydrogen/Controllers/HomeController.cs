using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hydrogen.Core.Domain.Multitenancy;
using Hydrogen.Infrastructure.Multitenancy;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Hydrogen.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationTenantResolver _tenantResolver;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(IApplicationTenantResolver tenantResolver, SignInManager<IdentityUser> signInManager)
        {
            _tenantResolver = tenantResolver;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var tenantContext = Request.HttpContext.GetTenantContext<ApplicationTenant>();

            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/signup");
            }

            if (tenantContext.Tenant.Hostnames == null)
            {
                return View(_tenantResolver.Tenants);
            }

            ViewBag.Claims = User.Claims;
            return View("Dashboard");           
        }

        [Route("/signup",Name= "Sign Up")]
        public IActionResult Signup()
        {
            var context = Request.HttpContext.GetTenantContext<ApplicationTenant>();
            if (context.Tenant.Hostnames == null)
            {
                return View(_tenantResolver.Tenants);
            }
            ViewData["Title"] = "Signup";
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
