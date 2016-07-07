using System.Collections.Generic;
using System.Linq;
using Helium.Console.Application;
using Helium.Console.Templates;

namespace Helium.Console.Commands.Default
{
    internal class HelpCommand : ConsoleCommand
    {
        public HelpCommand()
        {
            Name = "help";
            HelpName = "help";
            Aliases = new List<string> { "h" };
            Usage = "Shows a list of commands or help for one ConsoleCommand";
            Description = "Provides help for known commands.";
            ArgsUsage = "[ConsoleCommand]";

            Action = context =>
            {
                var args = context.Args();
                
                if(context.Get("help") == null && args.Count > 1)
                {
                    ShowCommandHelp(context, context.Application.GetCommand(args[1]));
                    return;
                }

                if (context.Get("help") != null && args.Any())
                {
                    ShowCommandHelp(context, context.Application.GetCommand(args[0]));
                    return;
                }

                ShowAppHelp(context);
            };
        }

        private void ShowCommandHelp(CommandContext context, ConsoleCommand consoleCommand)
        {
            if (consoleCommand != null)
            {
                var helpText = TemplateTransformer.TransformTemplate(consoleCommand, "CommandHelpTemplate");
                context.Application.Writer.WriteLine(helpText);
                return;
            }

            foreach (var helpCommand in context.Application.Commands)
            {
                if (helpCommand.HasName("help"))
                {
                    context.Application.Writer.WriteLine(TemplateTransformer.TransformTemplate(helpCommand, "CommandHelpTemplate"));
                }
            }            
        }

        private void ShowAppHelp(CommandContext context)
        {
            context.Application.Writer.WriteLine(TemplateTransformer.TransformTemplate(context.Application, "ApplicationHelpTemplate"));
        }

        #region SubcommandHelpTemplate

        const string SubcommandHelpTemplate = @"
var SubcommandHelpTemplate = `NAME:
   {{.HelpName}} - {{.Usage}}

USAGE:
   {{.HelpName}} ConsoleCommand{{if .VisibleFlags}} [ConsoleCommand options]{{end}} {{if .ArgsUsage}}{{.ArgsUsage}}{{else}}[arguments...]{{end}}

COMMANDS:{{range .Categories}}{{if .Name}}
  {{.Name}}{{ "":"" }}{{end}}{{range .Commands}}
    { {.Name} }
                { { with.ShortName} }, { {.} }
                { { end} }
                { { ""\t"" } }
                { {.Usage} }
                { { end} }
                { { end} }
                { { if .VisibleFlags} }
                OPTIONS:
                { { range.VisibleFlags} }
                { {.} }
                { { end} }
                { { end} }
`\n";
        #endregion
    }
}