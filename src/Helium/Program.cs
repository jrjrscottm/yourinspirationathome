using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Configuration;
using Helium.Actors;

namespace Helium
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Startup();

            //var consultant = Helium.SystemActors.Guardian.ActorOf(Props.Create(() => new Consultant()), "177ca29");

            //consultant.Tell(new Consultant.Registered("Scott", "AA-B1234"));
            //consultant.Tell(new Consultant.SayHello());
            //consultant.Tell(new Consultant.Recruited("18e5174"));
            //consultant.Tell(new Consultant.Recruited("f04178e"));
            Console.WriteLine("running. press 'q' to quit...");
            Console.ReadLine();
        }

        public static void Startup()
        {
            var config = ConfigurationFactory.ParseString(Configuration);
            SystemActors.Guardian = ActorSystem.Create("helium", config);
            SystemActors.Guardian.ActorOf(Props.Create(() => new Genealogy()), "genealogy");
            SystemActors.Guardian.ActorOf(Props.Create(() => new OrderRouter()), "orders");
        }

        private const string Configuration = @"
                akka {
                  actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    serializers {
                        wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
                    }
                        serialization-bindings {
                        ""System.Object"" = wire
                    }
                  }
                  remote {
                    helios.tcp {
                      port = 9999
                      hostname = localhost
                    }
                  }  
                }";
    }
}
