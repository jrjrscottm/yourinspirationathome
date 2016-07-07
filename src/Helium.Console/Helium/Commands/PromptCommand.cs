using Helium.Console.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helium.Console.Helium.Commands
{
    public class PromptCommand : ConsoleCommand
    {

        private readonly string Prompt = "helium>";
        public PromptCommand()
        {
            Name = "prompt";
            Aliases = new[] { "p" };
            Action = context =>
            {
                System.Console.Clear();
                System.Console.Out.WriteLine();
                System.Console.Out.Write(Prompt);
            };
        }
    }
}
