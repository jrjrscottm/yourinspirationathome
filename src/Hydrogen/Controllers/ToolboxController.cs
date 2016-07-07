using Hydrogen.Core.Domain.Subscriptions;

using Hydrogen.Services.Subscriptions;
using Hydrogen.ViewModels.Toolbox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Controllers
{
    [Authorize]
    public class ToolboxController : Controller
    {
        readonly ISubscriptionService _subscriptionService;
        readonly UserManager<IdentityUser> _userManager;

        public ToolboxController(
            UserManager<IdentityUser> userManager,
            ISubscriptionService subscriptionService)
        {
            _userManager = userManager;
            _subscriptionService = subscriptionService;
        }
        public IActionResult Index()
        {
            IEnumerable<Subscription> availableTools = _subscriptionService.GetAvailableSubscriptions();
            var userTools = _subscriptionService.GetActiveSubscriptionsByUserId(_userManager.GetUserId(User));

            var model = new ToolboxViewModel
            {
                Tools = from tool in availableTools
                        from userTool in userTools.Where( x=> x.SubscriptionId == tool.SubscriptionId).DefaultIfEmpty()
                        select new ToolSubscriptionOption
                        {
                            SubscriptionId = tool.SubscriptionId,
                            IsSubscribed = userTool != null,
                            Name = tool.DisplayName
                        }
            };

            return View(model);
        }
    }
}
