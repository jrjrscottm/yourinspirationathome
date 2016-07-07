using Hydrogen.Core.Domain.Consultants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Domain.Payments
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {

        }

        public int Id { get; set;}
        public string ConsultantId { get; set; }
        public string UserId { get; set; }

        public string Description { get; set; }
        public string ReferenceId { get; private set; }
        public bool IsActive { get; private set; }

        public void Activate(string externalRefId)
        {
            if (string.IsNullOrEmpty(externalRefId))
                throw new ArgumentNullException(nameof(externalRefId));

            ReferenceId = externalRefId;
            IsActive = true;
        }

        public Consultant Consultant { get; set; }
    }
}
