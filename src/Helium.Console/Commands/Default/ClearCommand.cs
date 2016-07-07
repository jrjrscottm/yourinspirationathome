using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helium.Console.Commands.Default
{
    public class ClearCommand : ConsoleCommand
    {
        public ClearCommand()
        {
            Name = "clear";
            Aliases = new[] {"cls"};
            Usage = "Clears the console.";
            ArgsUsage = "[ConsoleCommand]";
            Action = context =>
            {
                System.Console.Clear();
            };
        }
    }
}
