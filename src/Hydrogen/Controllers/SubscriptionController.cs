

using Hydrogen.Services.Payments;
using Hydrogen.Services.Subscriptions;
using Hydrogen.ViewModels.Subscription;
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
    public class SubscriptionController : Controller
    {
        readonly ISubscriptionService _subscriptionService;
        readonly UserManager<IdentityUser> _userManager;
        readonly ISubscriptionCommandHandler _commandHandler;
        readonly IPaymentService _paymentService;

        public SubscriptionController(
            ISubscriptionService subscriptionService,
            ISubscriptionCommandHandler commandHandler,
            IPaymentService paymentService,
            UserManager<IdentityUser> userManager)
        {
            this._paymentService = paymentService;
            this._commandHandler = commandHandler;
            this._userManager = userManager;
            this._subscriptionService = subscriptionService;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var subscriptions = _subscriptionService.GetActiveSubscriptionsByUserId(userId);

            var model = subscriptions.Select(x => new SubscriptionViewModel
            {
                Id = x.SubscriptionId,
                Name = x.DisplayName
            }).ToList();

            return View(model);
        }

        public IActionResult Subscribe(SubscriptionViewModel model)
        {

            return View();
        }

    }
}
