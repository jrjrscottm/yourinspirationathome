using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;

namespace Helium
{
    public static class SystemActors
    {
        public static ActorSystem Guardian;
        public static IActorRef Genealogy = ActorRefs.Nobody;
    }
}
