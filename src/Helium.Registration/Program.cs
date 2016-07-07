using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Helium.Registration.Actors;

namespace Helium.Registration
{
    public class Program
    {

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
}

akka.persistence{
  journal {
    plugin = ""akka.persistence.journal.sql-server""
    sql-server {
        class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
        schema-name = dbo
        auto-initialize = on
        connection-string = ""Server=(localdb)\\mssqllocaldb;Database=Helium.Database;Trusted_Connection=True;""
    }
}
snapshot-store{
    plugin = ""akka.persistence.snapshot-store.sql-server""
    sql-server {
        class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
        schema-name = dbo
        auto-initialize = on
        connection-string = ""Server=(localdb)\\mssqllocaldb;Database=Helium.Database;Trusted_Connection=True;""
    }
  }";
        public static void Main(string[] args)
        {

            var config = ConfigurationFactory.ParseString(Configuration);
            var system = ActorSystem.Create("helium", config);


            ActorPaths.ConsultantMasterActor = new ActorMetaData(system, ConsultantMasterActor.CreateProps(), "consultants");

            ActorPaths.ConsultantMasterActor.ActorRef.Tell(new ConsultantMasterActor.RegistrationSubmitted(1));


            var consultant = system.ActorOf(ConsultantActor.CreateProps(1),"consultant-1");

            consultant.Tell(new ConsultantActor.RegisterConsultant(1));

            


            Console.ReadKey();
        }
    }

    public class ActorMetaData
    {
        public ActorMetaData(ActorSystem system, Props props, string name, ActorMetaData parent = null)
            : this(name, parent)
        {
            ActorRef = system.ActorOf(props, name);
        }
        public ActorMetaData(string name, ActorMetaData parent = null)
        {
            Name = name;
            Parent = parent;
            // if no parent, we assume a top-level actor
            var parentPath = parent != null ? parent.Path : "/user";
            Path = $"{parentPath}/{Name}";
        }

        public string Name { get; private set; }

        public ActorMetaData Parent { get; set; }

        public string Path { get; private set; }

        public IActorRef ActorRef { get; set; }
    }

    public static class ActorPaths
    {
        public static ActorMetaData ConsultantMasterActor = new ActorMetaData("consultants");
    }
}
