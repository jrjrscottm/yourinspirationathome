using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Helium.Contracts.Members;
using Helium.Contracts.Orders;
using Helium.Contracts.Reports;

namespace Helium.Actors
{
    public class MemberActor : ReceiveActor
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MemberId { get; private set; }
        public string SponsorId { get; private set; }

        public MemberActor(string firstName, string lastName, string memberId, string sponsorId)
        {
            FirstName = firstName;
            LastName = lastName;
            MemberId = memberId;
            SponsorId = sponsorId;
            Orders = new List<AppliedOrder>();


            Receive<CreateChildMember>(createChildMember =>
            {
                var memberActorRef = Context.ActorOf(
                    Props.Create(
                        () => new MemberActor(
                            createChildMember.FirstName,
                            createChildMember.LastName,
                            createChildMember.MemberId,
                            createChildMember.SponsorId)), createChildMember.MemberId);

                createChildMember.Sender.Tell(new MemberReady(createChildMember.MemberId, memberActorRef.Path.ToStringWithAddress()));
            });

            Receive<ApplyOrder>(o =>
            {
                Orders.Add(new AppliedOrder(o.OrderId, o.Total, o.DateOrderProcessed));
            });

            ReceiveAsync<ReportTotalSales>(r =>
            {
                var totalSales = Orders.Where(x =>
                    x.DateProcessed > r.StartDate
                    && x.DateProcessed < r.EndDate)
                    .Sum(s => s.Total);

                foreach(var child in Context.GetChildren())
                {
                    child.Forward(r);
                }
                
                var message = new ReportTotalSales.SalesTotals(FirstName, LastName, MemberId, totalSales);
                r.RequestActor.Tell(message);
                return Task.FromResult(0);
            });
        }



        private List<AppliedOrder> Orders { get; set; }
    }

    public class AppliedOrder
    {
        public AppliedOrder(string orderId, decimal total, DateTime processed)
        {
            OrderId = orderId;
            Total = total;
            DateProcessed = processed;
            DateApplied = DateTime.UtcNow;
        }
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public DateTime DateProcessed { get; }
        public DateTime DateApplied { get; }
    }
}
