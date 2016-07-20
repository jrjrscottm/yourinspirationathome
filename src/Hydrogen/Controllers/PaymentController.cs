

using Hydrogen.Services;
using Hydrogen.Services.Payments;
using Hydrogen.ViewModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Hydrogen.Controllers
{
    public class PaymentController : Controller
    {
        readonly IPaymentCommandHandler _commandHandler;
        readonly IPaymentService _paymentService;
        readonly UserManager<IdentityUser> _userManager;

        public PaymentController(IPaymentService paymentService, IPaymentCommandHandler commandHandler, UserManager<IdentityUser> userManager)
        {
            _commandHandler = commandHandler;
            _userManager = userManager;
            _paymentService = paymentService;
        }

        [Authorize, HttpGet, Route("account/payment-details")]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var paymentMethod = _paymentService.GetPaymentMethodForUser(userId);

            var model = new PaymentMethodViewModel()
            {
                ClientToken = _paymentService.GetClientToken(),
                Name = paymentMethod?.Description
            };
            return View(model);
        }
    }
}
