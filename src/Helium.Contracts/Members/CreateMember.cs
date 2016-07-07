using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

namespace Helium.Contracts.Members
{
    public class CreateMember
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string SponsorId { get; }

        public CreateMember(string firstName, string lastName, string sponsorId)
        {
            FirstName = firstName;
            LastName = lastName;
            SponsorId = sponsorId;
        }
    }

    public class CreateChildMember : CreateMember
    {
        public string MemberId { get; }
        public IActorRef Sender { get; }

        public CreateChildMember(IActorRef sender, string memberId, CreateMember createMember)
            : this(sender, memberId, createMember.FirstName, createMember.LastName, createMember.SponsorId)
        {
            
        }

        public CreateChildMember(IActorRef sender, string memberId, string firstName, string lastName, string sponsorId) 
            : base(firstName, lastName, sponsorId)
        {
            Sender = sender;
            MemberId = memberId;
        }
    }
}
