using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helium.Distributors
{
    public class IdentityConvention
    {
        public static string CreateMemberId()
        {
            return "member-" + Guid.NewGuid().ToString("n").Substring(0, 7);
        }
    }
}
