using System.Collections.Generic;

namespace Helium.Distributors
{
    public class Downline
    {
        public Downline(Distributor distributor)
        {
            Distributor = distributor;
            Members = new List<Distributor>();
        }

        public Downline()
        {
            Members = new List<Distributor>();
        }

        public Distributor Distributor { get; set; }
        
        public List<Distributor> Members { get; set; }
    }
}