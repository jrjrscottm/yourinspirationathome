using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Helium.Console.Commands;
using Helium.Console.Flag.TypedFlags;
using Helium.Console.Helium.Commands.Member;
using Helium.Console.Helium.Commands.Orders;
using Helium.Console.Helium.Commands.Report;
using Helium.Console.Helium.Commands.Tell;
using Helium.Console.Templates;
using Helium.Console.Helium.Commands;

namespace Helium.Console.Helium
{
    public sealed class Helium : Application.Application
    {
        public static string GetGenealogyBasePath(CommandContext context, string path = "")
        {
            var addr = context.Get("addr");
            var system = context.Get("system");
            var actorPath = path ?? context.Get("path");

            return GetBasePath(context,$"genealogy/{actorPath}");
        }

        public static string GetBasePath(CommandContext context, string path = "")
        {
            var addr = context.Get("addr");
            var system = context.Get("system");
            var actorPath = path ?? context.Get("path");

            return $"akka.tcp://{system}@{addr}/user/{actorPath}";
        }
        private Helium() { }

        static Helium()
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
            port = 10000
            hostname = localhost
        }
    }
}");
            ActorSystem = ActorSystem.Create("HeliumClient", config);
        }


        public static ActorSystem ActorSystem;

        public override void Dispose()
        {
            ActorSystem?.Dispose();

            base.Dispose();
        }

        public static Helium Create()
        {
            return new Helium
            {
                Name = "Helium",
                UsageText = "helium>",
                Usage = "A cli application for the Helium platform.",
                ArgsUsage = "",
                Flags = new List<Flag.Flag>
            {
                new StringFlag
                {
                    Name = "addr",
                    Usage = "the remote system address",
                    DefaultValue = "localhost:9999"
                },
                new StringFlag
                {
                    Name = "system",
                    Usage = "the Actor system name",
                    DefaultValue = "helium"
                },
                new StringFlag
                {
                    Name = "path",
                    Usage = "the path of the actor",
                    DefaultValue = "177ca29"
                }
            },
                Commands = new List<ConsoleCommand>
                {
                    new RegisterCommand(),
                    new OrdersCommand(),
                    new ReportSalesCommand(),
                    new PromptCommand(),
                    new ConsoleCommand
                    {
                        Name ="exit",
                        ShortName = "q",
                        Action = context =>
                        {
                            Environment.Exit(0);
                        }
                    }
                 }
            };
        }
    }
}
