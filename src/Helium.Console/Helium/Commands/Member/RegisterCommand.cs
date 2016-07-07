using System.Collections.Generic;
using System.IO;
using Akka.Actor;
using Helium.Console.Commands;
using Helium.Console.Flag.TypedFlags;
using Helium.Contracts.Members;

namespace Helium.Console.Helium.Commands.Member
{
    public class RegisterCommand : ConsoleCommand
    {
        public RegisterCommand()
        {
            Name = "register";
            Aliases = new[] {"reg"};
            Flags = new List<Flag.Flag>
            {
                new StringFlag
                {
                    Name = "first-name, first",
                    Usage = "The member's first name"
                },
                new StringFlag
                {
                    Name = "last-name, last",
                    Usage = "The member's last name"
                },
                new StringFlag
                {
                    Name = "sponsor",
                    Usage = "The member's sponsor ID."
                }
            };
            Action = context =>
            {
                var firstName = context.Get("first-name");
                var lastName = context.Get("last-name");
                var sponsorId = context.Get("sponsor");

                Helium.ActorSystem.ActorOf(
                    Props.Create(() => new RegisterActor(context, firstName, lastName, sponsorId)));
            };
        }

        private class RegisterActor : ReceiveActor
        {
            private RegisterActor(TextWriter output, CommandContext context, string firstName, string lastName, string sponsorId)
            {
                var members = Helium.ActorSystem.ActorSelection(Helium.GetGenealogyBasePath(context, "members"));
                members.Tell(new CreateMember(firstName, lastName, sponsorId));

                Receive<MemberReady>(ready =>
                {
                    output.WriteLine($"Member {ready.MemberId} ready at {ready.MemberPath}");
                    output.WriteLine("");
                    output.Write("helium>");
                });
            }

            public RegisterActor(CommandContext context, string firstName, string lastName, string sponsorId)
                : this(System.Console.Out, context, firstName, lastName, sponsorId)
            {
            }
        }
    }
}
