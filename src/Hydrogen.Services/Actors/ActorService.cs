using Akka.Actor;
using Akka.Configuration;
using Helium.Contracts.Members;

namespace Hydrogen.Services.Actors
{
    public class ActorService
    {
        public static ActorSystem System { get; private set; }

        public ActorService()
        {
            var config = ConfigurationFactory.ParseString(@"
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
            port = 10001
            hostname = localhost
        }
    }
}");
            System = ActorSystem.Create("HydrogenClient", config);
        }

        public void OnMemberRegistered(string firstName, string lastName, string sponsorId = null)
        {
            var actorRef = System.ActorSelection(GetGenealogyBasePath("members"));
            actorRef.Tell(new CreateMember(firstName, lastName, sponsorId));
        }

        private string _address;
        private string _system;

        private string GetGenealogyBasePath(string path)
        {
            return GetBasePath($"genealogy/{path}");
        }

        private string GetBasePath(string path = "")
        {
            return $"akka.tcp://{_system}@{_address}/user/{path}";
        }
    }
}
