using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Helium.Contracts.Orders;

namespace Helium.Actors
{
    public class OrderRouter : ReceiveActor
    {
        public OrderRouter()
        {
            Receive<OrderProcessed>(m => HandleOrderProcessed(m));
        }

        public void HandleOrderProcessed(OrderProcessed message)
        {
            Context.ActorSelection($"/user/genealogy/members/{message.MemberId}")
                .Tell(new ApplyOrder (message.OrderId, message.Total, message.DateProcessed));
        }
    }
}
