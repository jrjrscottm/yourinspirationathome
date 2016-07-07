namespace Helium.Console.Commands.Print
{
    public class ExamplePrintCommand : PrintCommand
    {
        private readonly string[] _args;

        public ExamplePrintCommand(params string[] args)
        {
            _args = args;
        }

        public override string GetText()
        {
            return $"The arguments passed were: {string.Join(", ", _args)}";
        }
    }
}