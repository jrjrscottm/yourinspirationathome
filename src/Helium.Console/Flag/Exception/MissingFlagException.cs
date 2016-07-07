namespace Helium.Console.Flag.Exception
{
    public class MissingFlagException : System.Exception
    {
        public MissingFlagException(string flag)
            :base($"No such flag '{flag}'")
        {
            Flag = flag;
        }

        public string Flag { get; set; }
    }
}