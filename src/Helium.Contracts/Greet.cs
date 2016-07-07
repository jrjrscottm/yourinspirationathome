using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helium.Contracts
{
    public class Greet
    {
        public string Message { get; }

        public Greet(string message)
        {
            Message = message;
        }
    }
}
