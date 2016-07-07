using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helium.Contracts.Orders
{
    public class OrderProcessed
    {
        public OrderProcessed(string orderId, string memberId, decimal total)
        {
            OrderId = orderId;
            MemberId = memberId;
            Total = total;
            DateProcessed = DateTime.UtcNow;
        }
        public string OrderId { get; }
        public string MemberId { get; }
        public decimal Total { get; }
        public DateTime DateProcessed { get; }
    }

    public class ApplyOrder
    {
        public ApplyOrder(string orderId, decimal total, DateTime dateProcessed)
        {
            OrderId = orderId;
            Total = total;
            DateOrderProcessed = dateProcessed;

        }
        public string OrderId { get; }
        public decimal Total { get; }
        public DateTime DateOrderProcessed { get; }
    }
}
