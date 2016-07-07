using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

namespace Helium.Registration.Actors
{
    public class ConsultantMasterActor : ReceiveActor
    {
        public static Func<Props> CreateProps = () => Props.Create(() => new ConsultantMasterActor());
        public class RegistrationSubmitted
        {
            public RegistrationSubmitted(int consultantId)
            {
                ConsultantId = consultantId;
            }
            public int ConsultantId { get; }
        }

        public ConsultantMasterActor()
        {
            Receive<RegistrationSubmitted>(m =>
            {
                Context.ActorOf(ConsultantActor.CreateProps(m.ConsultantId), $"consultant-{m.ConsultantId}");
            });
        }
    }
}
