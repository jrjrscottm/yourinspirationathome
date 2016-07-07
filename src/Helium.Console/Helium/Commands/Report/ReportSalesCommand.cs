using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Helium.Console.Commands;
using Helium.Console.Flag.TypedFlags;
using Helium.Contracts.Reports;

namespace Helium.Console.Helium.Commands.Report
{
    public class ReportSalesCommand : ConsoleCommand
    {
        public ReportSalesCommand()
        {
            Name = "report";
            Aliases = new[] {"rep"};
            Flags = new List<Flag.Flag>
            {
                new StringFlag
                {
                    Name = "member-id",
                    Usage = "The member Id to get a report for"
                }
            };
            Subcommands = new List<ConsoleCommand>
            {
                new ConsoleCommand
                {
                    Name = "totals",
                    Aliases = new List<string> {"total"},
                    Action = context =>
                    {
                        var memberId = context.Get("member-id");

                        Helium.ActorSystem.ActorOf(
                            Props.Create(
                                () => new TotalsReportActor(
                                        context, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow,memberId)));
                    }
                }
            };
        }


        public class TotalsReportActor : ReceiveActor
        {
            public string MemberId { get; }

            public TotalsReportActor(CommandContext commandContext, DateTime startDate, DateTime endDate, string memberId = null)
            {
                MemberId = memberId;

                Context.ActorSelection(Helium.GetGenealogyBasePath(commandContext, "members/*"))
                    .Tell(new ReportTotalSales(Context.Self, startDate, endDate, memberId));

                Receive<ReportTotalSales>(report =>
                {
                    var context = Context;
                    var t = Task.Run(async () =>
                    {
                        var tasks = new List<Task<ReportTotalSales.SalesTotals>>();
                        if (report.MemberId == null)
                        {
                            foreach (IActorRef actorRef in context.GetChildren())
                            {
                                tasks.Add(actorRef.Ask<ReportTotalSales.SalesTotals>(report));
                            }
                        }
                        else
                        {
                            tasks.Add(context.Child(report.MemberId).Ask<ReportTotalSales.SalesTotals>(report));
                        }

                        await Task.WhenAll(tasks);

                        var totals = new List<ReportTotalSales.SalesTotals>();
                        foreach (var task in tasks)
                        {
                            totals.Add(task.Result);
                        }

                        return new ReportTotalSales.SalesTotalsResponse(totals);
                    });

                    t.PipeTo(Sender, Self);

                });

                Receive<ReportTotalSales.SalesTotals>(m =>
                {
                    Self.Tell(new ReportTotalSales.SalesTotalsResponse(new List<ReportTotalSales.SalesTotals>() { m }));
                });

                Receive<ReportTotalSales.SalesTotalsResponse>(m =>
                {
                    foreach (var respone in m.Totals)
                    {
                        commandContext.Application.Writer.WriteLine($"\r[{respone.MemberId}] {respone.LastName}, {respone.FirstName}\t\t{respone.Total:C}");
                    }
                });
            }
        }
    }
}
