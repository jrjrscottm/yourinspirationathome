using Hydrogen.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Hydrogen.ViewModels.Payment
{
    public class PaymentMethodViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Expiration { get; set; }
        public string ClientToken { get; set; }

    }

}
