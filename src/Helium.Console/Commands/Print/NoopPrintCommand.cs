namespace Helium.Console.Commands.Print
{
    public class NoopPrintCommand : PrintCommand
    {
        public override string GetText()
        {
            return "";
        }
    }
}