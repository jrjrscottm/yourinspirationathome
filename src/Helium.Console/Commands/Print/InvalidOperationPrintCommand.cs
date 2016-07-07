namespace Helium.Console.Commands.Print
{
    public class InvalidOperationPrintCommand : PrintCommand
    {
        public override string GetText()
        {
            return "error: unknown ConsoleCommand";
        }
    }
}