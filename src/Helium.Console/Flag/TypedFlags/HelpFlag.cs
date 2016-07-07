namespace Helium.Console.Flag.TypedFlags
{
    public class HelpFlag : BoolFlag
    {
        public HelpFlag()
        {
            Name = "help, h";
            Usage = "show help";
            DefaultValue = false;
        }
    }
}