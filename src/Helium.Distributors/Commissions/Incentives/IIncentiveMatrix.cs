using System.Collections.Generic;

namespace Helium.Distributors.Commissions.Data.Models
{
    public interface IIncentiveMatrix
    {
        decimal[,] Values { get; set; }
    }

    public interface IIncentiveTierValue<T, TK>
    {
        Dictionary<T, TK> Values { get; set; }
    }
}