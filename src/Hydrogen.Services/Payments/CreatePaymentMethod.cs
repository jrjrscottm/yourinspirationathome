using Hydrogen.Core.Commands;
using Hydrogen.Infrastructure.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Services.Payments
{

    public class CreateUserPaymentMethod : ICommand
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostalCode { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string SecurityCode { get; set; }
        public string ReferenceId { get; set; }
    }

    public class CreatePaymentMethodResult : ResultBase
    {
        public CreatePaymentMethodResult(string erroMessage = null) : base(erroMessage)
        {
        }
    }
}
