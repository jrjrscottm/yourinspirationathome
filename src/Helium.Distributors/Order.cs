using System;
using System.Collections.Generic;
using Helium.Distributors.Commissions;

namespace Helium.Distributors
{
    public class Order
    {
        public DateTime DatePlaced { get; set; }
        public List<ICommissionableItem> OrderItems { get; set; }

        public decimal SubTotal { get; set; }
    }
}