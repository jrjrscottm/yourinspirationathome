using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Core.Exceptions
{
    public class UnknownUserException : Exception
    {
        public UnknownUserException(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; private set; }
    }
}
