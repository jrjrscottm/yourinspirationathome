using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Helium.Console.Commands;
using Helium.Console.Commands.Default;
using Helium.Console.Flag;
using Helium.Console.Flag.TypedFlags;
using Helium.Console.Templates;

namespace Helium.Console.Application
{
    public class Application : IDisposable
    {
        internal static HelpCommand helpCommand = new HelpCommand();
        internal static HelpFlag helpFlag = new HelpFlag();

        internal static VersionCommand versionCommand = new VersionCommand();
        internal static ClearCommand clearCommand = new ClearCommand();

        public Application()
        {
            Name = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            HelpName = Name;
            Usage = "A new cli application";
            UsageText = "";
            Version = "0.0.0";
            Action = helpCommand.Action;
            Writer = System.Console.Out;
        }

        private Application(Application application, ConsoleCommand consoleCommand, List<ConsoleCommand> subcommands)
        {
            Name = application.Name;
            HelpName = application.HelpName;
            Usage = application.Usage;
            UsageText = application.UsageText;
            ArgsUsage = application.ArgsUsage;
            Author = application.Author;
            Email = application.Email;
            Version = application.Version;
            HideHelp = application.HideHelp;
            HideVersion = application.HideVersion;
            Authors = application.Authors;
            Commands = subcommands;
            Flags = consoleCommand.Flags;
            Writer = application.Writer;
            //TODO: Subcommand help action
            Action = consoleCommand.Action ?? helpCommand.Action;

        }

        

        public string Name { get; set; }
        public string HelpName { get; set; }
        public string Usage { get; set; }
        public string UsageText { get; set; }
        public string ArgsUsage { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Version { get; set; }
        public bool HideHelp { get; set; }
        public bool HideVersion { get; set; }
        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<ConsoleCommand> Commands { get; set; } = new List<ConsoleCommand>();
        public ICollection<Flag.Flag> Flags { get; set; } = new List<Flag.Flag>();
        public Action<CommandContext> Action { get; set; }
        public TextWriter Writer { get; set; }

        public void Run(params string[] args)
        {
            if (!string.IsNullOrEmpty(Author) || !string.IsNullOrEmpty(Email))
            {
                Authors.Add(new Author {Name = Author, Email = Email});
            }

            foreach (var command in Commands)
            {
                if (string.IsNullOrEmpty(command.HelpName))
                {
                    command.HelpName = $"{HelpName} {command.Name}";
                }
            }

            if (this.GetCommand(clearCommand.Name) == null)
            {
                Commands.Add(clearCommand);
            }

            if (this.GetCommand(helpCommand.Name) == null && !HideHelp)
            {
                Commands.Add(helpCommand);

                if (!this.HasFlag(helpFlag))
                {
                    Flags.Add(helpFlag);
                }
            }

            if(this.GetCommand(versionCommand.Name) == null && !HideVersion)
            {
                Commands.Add(versionCommand);
            }

            var flagSet = new FlagSet(Name);

            foreach (var flag in Flags)
            {
                flag.Apply(flagSet);
            }

            flagSet.Parse(args.ToArray());
            Flags.NormalizeFlags(flagSet);

            var context = new CommandContext(this, flagSet);
            var ctxArgs = context.Args();

            if (ctxArgs.Any())
            {
                var name = args.First();

                var c = this.GetCommand(name);
                if (c != null)
                {
                    if (c.Flags.Any())
                    {
                        foreach (var flag in c.Flags)
                        {
                            flag.Apply(flagSet);
                        }

                        flagSet.Parse(context.Args().ToArray());
                        Flags.NormalizeFlags(flagSet);
                    }

                    context.ConsoleCommand = c;
                    if (c.Subcommands.Any())
                    {
                        
                        new Application(this, c, c.Subcommands).RunAsSubcommand(context);
                        return;
                    }                  

                    if (!CheckHelp(context, c)) c?.Action(context);
                }
            }
            else
            {
                Action(context);
            }           
        }

        private void RunAsSubcommand(CommandContext context)
        {
            if (this.GetCommand(helpCommand.Name) == null && !HideHelp)
            {
                Commands.Add(helpCommand);
                if (!this.HasFlag(helpFlag))
                {
                    Flags.Add(helpFlag);
                }
            }

            var flagSet = new FlagSet(Name);

            foreach (var flag in Flags)
            {
                flag.Apply(flagSet);
            }

            flagSet.Parse(context.Args().Skip(1).ToArray());
            Flags.NormalizeFlags(flagSet);

            var subContext = new CommandContext(this, flagSet, context);
            //var ctxArgs = subContext.Args();
            var ctxArgs = subContext.Args();

            if (ctxArgs.Any())
            {
                var name = ctxArgs.First();
                var c = this.GetCommand(name);
                if (c != null)
                {
                    if (c.Flags.Any())
                    {
                        foreach (var flag in c.Flags)
                        {
                            flag.Apply(flagSet);
                        }

                        flagSet.Parse(subContext.Args().ToArray());
                        Flags.NormalizeFlags(flagSet);
                    }
                    subContext.ConsoleCommand = c;
                    if (!CheckHelp(subContext, c)) c.Action(subContext);
                    return;
                }
                else
                {
                    flagSet.Set("help", true);
                    CheckHelp(subContext, context.ConsoleCommand);
                    return;
                }
            }

            Action(context);
        }

        private bool CheckHelp(CommandContext context, ConsoleCommand consoleCommand)
        {
            if (context.Get("help") == null || !context.Get<bool>("help")) return false;

            context.Application.Writer.WriteLine(TemplateTransformer.TransformTemplate(consoleCommand, "CommandHelpTemplate"));

            return true;
        }

        public virtual void Dispose()
        {
            
        }
    }
}