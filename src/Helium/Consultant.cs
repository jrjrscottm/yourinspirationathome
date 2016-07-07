using System;
using Akka.Actor;
using Helium.Contracts;

namespace Helium
{
    public class Consultant : ReceiveActor
    {
        public class Registered
        {
            public Registered(string name, string distributorId)
            {
                Name = name;
                DistributorId = distributorId;
            }
            public string Name { get; }
            public string DistributorId { get; }
        }

        public class Recruited
        {
            public Recruited(string distributorId)
            {
                DistributorId = distributorId;
            }
            public string DistributorId { get; }
        }

        public class SayHello
        {
            
        }

        public string Name { get; set; }
        public string DistributorId { get; set; }

        public Consultant()
        {
            Receive<Registered>(registration =>
            {
                Name = registration.Name;
                DistributorId = registration.DistributorId;
            });

            Receive<SayHello>(_ =>
            {
                Console.WriteLine($"Hello, my name is {Name} [{DistributorId}]");
            });

            Receive<Recruited>(newRecruit =>
            {
                var actor = Context.ActorOf<Consultant>(newRecruit.DistributorId);
                Console.WriteLine($"[{DistributorId}] Recruited {actor.Path}");
            });

            Receive<Greet>(g =>
            {
                Console.WriteLine($"Received message: {g.Message}");
            });
        }
    }
}