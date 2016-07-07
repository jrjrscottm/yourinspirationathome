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
    public interface IAddPaymentMethodHandler : ICommandHandler<CreateUserPaymentMethod, CreatePaymentMethodResult>
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

        public CreatePaymentMethodResult Handle(CreateUserPaymentMethod command)
        {
            try
            {
                var consultant = _context.Consultants.SingleOrDefault(x => x.UserId == command.UserId);

                if(consultant == null)
                {
                    _log.Error("Unable to find consultant with ID {consultantId}", command.UserId);

                    throw new UnknownUserException(command.UserId);
                }

                if (string.IsNullOrEmpty(command.ReferenceId))
                {
                    command.ReferenceId = _paymentService.AuthorizeCreditCard(
                            consultant.UserId,
                            command.CardNumber,
                            command.ExpirationMonth,
                            command.ExpirationYear,
                            command.SecurityCode
                        );
                }

                var paymentMethod = new Domain.Payments.PaymentMethod()
                {
                    Description = $"CC: VISA ending in 1111",
                    UserId = consultant.UserId,
                    ConsultantId = consultant.ConsultantId
                };
                
                paymentMethod.Activate(command.ReferenceId);

                if(consultant.PaymentMethods == null)
                {
                    consultant.PaymentMethods = new List<Domain.Payments.PaymentMethod>() { paymentMethod };
                } else
                {
                    consultant.PaymentMethods.Add(paymentMethod);
                }

                _context.SaveChanges();

                return new CreatePaymentMethodResult();
            }
            catch(ArgumentNullException e)
            {
                _log.Error(e, "Unable to create payment method.");
                return new CreatePaymentMethodResult("Unable to create payment method.");
            }
            catch(Exception e)
            {
                Log.Error(e, "An unknown error occured");
                return new CreatePaymentMethodResult("An unknown error occurred.");
            }
        }
    }


    public interface IPaymentAuthorizationService
    {
        string GetClientToken();
        string AuthorizeCreditCard(string customerId, string cardNumber, int expMonth, int expYear, string securityCode);
        
    }

    public interface IPaymentService : IPaymentAuthorizationService
    {
        Domain.Payments.PaymentMethod GetPaymentMethodForUser(string id);
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

        public string GetClientToken()
        {
            return _gateway.ClientToken.generate();
        }

        public Domain.Payments.PaymentMethod GetPaymentMethodForUser(string id)
        {
            return _context.PaymentMethods.SingleOrDefault(x => x.UserId == id);
        }
    }
}
