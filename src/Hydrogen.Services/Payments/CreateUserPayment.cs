using Hydrogen.Core.Commands;
using Hydrogen.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Services.Payments
{
    public class CreateUserPaymentAccount : ICommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ConsultantId { get; set; }
        public string UserId { get; set; }
        public string PaymentMethodId { get; set; }
    }

    public class CreateUserPaymentAccountResult : ResultBase
    {
        public CreateUserPaymentAccountResult(string erroMessage = null) : base(erroMessage)
        {
        }
    }
}
