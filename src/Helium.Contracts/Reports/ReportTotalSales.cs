using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helium.Contracts.Reports
{
    public class ReportTotalSales
    {

        public ReportTotalSales(IActorRef requestActor, DateTime startDate, DateTime endDate, string memberId = null)
        {
            RequestActor = requestActor;
            StartDate = startDate;
            EndDate = endDate;
            MemberId = memberId;
        }

        public string MemberId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public IActorRef RequestActor { get; }

        public class SalesTotalsResponse
        {
            public SalesTotalsResponse(List<SalesTotals> totals)
            {
                Totals = totals;
            }
            public List<SalesTotals> Totals { get; }
        }
        public class SalesTotals
        {
            public SalesTotals(string firstName, string lastName, string memberId, decimal total)
            {
                FirstName = firstName;
                LastName = lastName;
                MemberId = memberId;
                Total = total;
            }
            public string FirstName { get; }
            public string LastName { get; }
            public decimal Total { get; }
            public string MemberId { get; }
        }
    }
}
