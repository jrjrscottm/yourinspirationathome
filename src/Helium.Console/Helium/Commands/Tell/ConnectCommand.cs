using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Helium.Console.Commands;
using Helium.Console.Flag.TypedFlags;
using Helium.Contracts;
namespace Helium.Console.Helium.Commands.Tell
{
    public class ConnectCommand : ConsoleCommand
    {
        public ConnectCommand()
        {
            Name = "connect";
            Usage = "Connect to remote actor system.";
            HelpName = "connect";
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
            };
        }
    }
}
