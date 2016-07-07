using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helium.Console.Commands;
using Helium.Console.Flag.TypedFlags;
using Helium.Contracts.Orders;

namespace Helium.Console.Helium.Commands.Orders
{
    public class OrdersCommand : ConsoleCommand
    {
        public OrdersCommand()
        {
            Name = "orders";
            Aliases = new List<string> {"sales"};
            Subcommands = new List<ConsoleCommand>
            {
                new ProcessOrderCommand()
            };
        }
    }

    public class ProcessOrderCommand : ConsoleCommand
    {
        public ProcessOrderCommand()
        {
            Name = "process";
            Aliases = new List<string> {"p"};
            Flags = new List<Flag.Flag>
            {
                new DecimalFlag
                {
                    Name="total",
                    Usage = "The total order amount"
                },
                new StringFlag
                {
                    Name="member",
                    Usage = "The member ID of whom to apply the order"
                }
            };
            Action = context =>
            {
                var total = context.Get<decimal>("total");
                var member = context.Get("member");
                var orderId = $"order-{Guid.NewGuid().ToString("n").Substring(0, 7)}";

                Helium.ActorSystem.ActorSelection(Helium.GetBasePath(context, "orders"))
                    .Tell(new OrderProcessed(orderId, member, total));
            };
        }
    }
}

