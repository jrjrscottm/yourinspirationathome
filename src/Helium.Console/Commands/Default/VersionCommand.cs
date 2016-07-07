using System.Collections.Generic;

namespace Helium.Console.Commands.Default
{
    public class VersionCommand : ConsoleCommand
    {
        public VersionCommand()
        {
            Name = "version";
            Aliases = new List<string> {"v"};
            Usage = "print the version";
            Action = context =>
            {
                if (!context.Application.HideVersion)
                {
                    context.Application.Writer.WriteLine($"v{context.Application.Version}");
                }
            };
        }
        
    }
}