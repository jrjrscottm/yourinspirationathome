using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hydrogen.Core.Domain.Multitenancy;
using Hydrogen.Infrastructure.Multitenancy;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Hydrogen.Services.VideoStore;
using Hydrogen.ViewModels.VideoStore;
using System.Linq;

namespace Hydrogen.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationTenantResolver _tenantResolver;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IVideoStoreService _videoStoreService;

        public HomeController(IApplicationTenantResolver tenantResolver,
        IVideoStoreService videoStoreService,    
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
        {
            _tenantResolver = tenantResolver;
            _signInManager = signInManager;
            _userManager = userManager;
            _videoStoreService = videoStoreService;
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

            var videos = _videoStoreService.GetUserVideos(_userManager.GetUserId(User));

            var model = new VideoStoreViewModel
            {
                Videos = videos.Select(v => new VideoViewModel
                {
                    ImagePath = v.ImagePath,
                    EmbedCode = v.EmbedCode,
                    Url = v.Url,
                    Name = v.Title,
                    Description = v.Description
                }).ToList()
            };

            return View("Dashboard", model);           
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
