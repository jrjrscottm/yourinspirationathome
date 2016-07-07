using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Helium.Contracts.Members;

namespace Helium.Actors
{
    public class Genealogy : ReceiveActor
    {
        protected IActorRef CommissionsMasterActorRef;

        protected IActorRef MembersMasterActorRef;

        public const string CommissionableActorName = "commissionable";
        public const string MembersActorName = "members";

        public Genealogy()
        {
            Initializing();
        }

        private void Initializing()
        {
            if (Context.Child(CommissionableActorName).Equals(ActorRefs.Nobody))
            {
                CommissionsMasterActorRef = Context.ActorOf(
                    Props.Create(() => new CommissionsMaster()), CommissionableActorName);
            }
            else
            {
                CommissionsMasterActorRef = Context.Child(CommissionableActorName);
            }

            if (Context.Child(MembersActorName).Equals(ActorRefs.Nobody))
            {
                MembersMasterActorRef = Context.ActorOf(
                    Props.Create(() => new MembersMaster()), MembersActorName);
            }
            else
            {
                MembersMasterActorRef = Context.Child(MembersActorName);
            }


            Become(Ready);
        }

        private void Ready()
        {
            Receive<MemberRegistered>(createMember =>
            {
                MembersMasterActorRef.Tell(
                    new CreateMember(createMember.FirstName, createMember.LastName, createMember.SponsorId));
            });
        }

    }
}
