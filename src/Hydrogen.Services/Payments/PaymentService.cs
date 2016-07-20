using System;

using Hydrogen.Data;
using Hydrogen.Infrastructure.Commands;
using System.Linq;
using Hydrogen.Domain.Payments;
using Serilog;
using Braintree;
using System.Collections.Generic;
using Hydrogen.Core.Exceptions;
//using Braintree;

namespace Hydrogen.Services.Payments
{
    public interface IAddPaymentMethodHandler : 
        ICommandHandler<CreateUserPaymentAccount, CreateUserPaymentAccountResult>
    {
    }

    public interface IPaymentCommandHandler : IAddPaymentMethodHandler
    {
        
    }

    public class PaymentCommandHandler : IPaymentCommandHandler
    {
        readonly IPaymentService _paymentService;
        readonly HydrogenApplicationContext _context;
        readonly ILogger _log;

        public PaymentCommandHandler(
            IPaymentService paymentService, 
            HydrogenApplicationContext context,
            ILogger log)
        {
            _context = context;
            _paymentService = paymentService;
            _log = log;
        }

        public CreateUserPaymentAccountResult Handle(CreateUserPaymentAccount command)
        {
            throw new NotImplementedException();
        }
    }

    public interface IHostedPaymentService
    {
        string GetClientToken(string customerId = null);
    }

    public interface IPaymentAuthorizationService
    {
        string AuthorizeCreditCard(string customerId, string cardNumber, int expMonth, int expYear, string securityCode);
        
    }

    public interface IPaymentService : IPaymentAuthorizationService, IHostedPaymentService
    {
        Domain.Payments.PaymentMethod GetPaymentMethodForUser(string id);
        bool CreatePaymentAccount(string userId, string paymentMethodNonce);
        bool CreatePaymentAccountSubscription(string userId, string distributorId, string firstName, string lastName, string email, string paymentMethodNonce);
    }

    public class BraintreePaymentService : IPaymentService
    {
        private IBraintreeGateway _gateway;
        private readonly HydrogenApplicationContext _context;
        private readonly ILogger _log;

        public BraintreePaymentService(IBraintreeGateway gateway, HydrogenApplicationContext context, ILogger log)
        {
            _gateway = gateway;
            _context = context;
            _log = log;
        }

        public bool CreatePaymentAccount(string userId, string paymentMethodNonce)
        {
            var customerRequest = new CustomerRequest()
            {
                CustomerId = userId,
                PaymentMethodNonce = paymentMethodNonce
            };

            var result = _gateway.Customer.Create(customerRequest);

            if(result.IsSuccess())
            {
                return true;
            }

            foreach(var error in result.Errors.All())
            {
                _log.Error("Unable to create payment account for {userId}: {code} {message}", userId, error.Code, error.Message);
            }
            return false;
        }

        public bool CreatePaymentAccountSubscription(string userId, string distributorId, string firstName, string lastName, string email, string paymentMethodNonce)
        {
            var customerRequest = new CustomerRequest
            {
                CustomerId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PaymentMethodNonce = paymentMethodNonce,
                CustomFields = new Dictionary<string, string>
                {
                    { "distributorid", distributorId }
                }
            };

            var createCustomerResult = _gateway.Customer.Create(customerRequest);

            if(createCustomerResult.IsSuccess())
            {
                var subscriptionRequest = new SubscriptionRequest()
                {
                    PaymentMethodToken = createCustomerResult.Target.PaymentMethods[0].Token,
                    PlanId = "subscription-videostore"
                };

                var createSubscriptionResult = _gateway.Subscription.Create(subscriptionRequest);

                if (createSubscriptionResult.IsSuccess())
                {
                    return true;
                }

                _log.Error("Unable to create subscription with BrainTree for {consultantId}. {message}", userId, createSubscriptionResult.Message);
            }

            _log.Error("Unable to create customer with BrainTree for {consultantId}. {message}", userId, createCustomerResult.Message);
            return false;
        }

        public string AuthorizeCreditCard(string customerId, string cardNumber, int expMonth, int expYear, string securityCode)
        {
            var response = _gateway.PaymentMethod.Create(
                new PaymentMethodRequest()
                {
                    CustomerId = customerId,
                    Number = cardNumber,
                    ExpirationMonth = expMonth.ToString(),
                    ExpirationYear = expYear.ToString(),
                    CVV = securityCode,
                });

            if(!response.IsSuccess())
            {
                _log.Error("Unable to create payment method with BrainTree for {consultantId}. {message}", customerId, response.Message);
                return null;
            }

            Log.Information("Added payment method with BrainTree for {consultantId}. {message}", response.Message);
            return response.Target.Token;
        }

        public string GetClientToken(string userId = null)
        {
            if (userId != null)
            {
                return _gateway.ClientToken.generate(new ClientTokenRequest
                {
                    CustomerId = userId
                });
            }
            return _gateway.ClientToken.generate();
        }

        public Domain.Payments.PaymentMethod GetPaymentMethodForUser(string id)
        {
            return _context.PaymentMethods.SingleOrDefault(x => x.UserId == id);
        }
    }
}
