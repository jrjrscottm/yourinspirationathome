using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using Helium.Contracts.Members;
using Helium.Contracts.Reports;

namespace Helium.Actors
{
    public class MembersMaster :ReceiveActor
    {
        public MembersMaster()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<CreateMember>(createMember =>
            {
                var memberId = Guid.NewGuid().ToString("n").Substring(0, 7);

                IActorRef sponsorActorRef = null;

                if (!string.IsNullOrEmpty(createMember.SponsorId))
                {
                    try
                    {
                        sponsorActorRef = Context.ActorSelection($"../*/{createMember.SponsorId}")
                            .ResolveOne(TimeSpan.FromSeconds(3)).Result;
                    }
                    catch (AggregateException)
                    {
                        
                    }
                }

                if (sponsorActorRef == null)
                {
                    var memberActorRef = Context.ActorOf(
                        Props.Create(
                            () => new MemberActor(
                                createMember.FirstName,
                                createMember.LastName,
                                memberId,
                                createMember.SponsorId)), memberId);

                    Sender.Tell(new MemberReady(memberId, memberActorRef.Path.ToStringWithAddress()));
                }
                else
                {
                    sponsorActorRef.Tell(new CreateChildMember(Sender,memberId, createMember));
                }
            });
        }
    }
}
