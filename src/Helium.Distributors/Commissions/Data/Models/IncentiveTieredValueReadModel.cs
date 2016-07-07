using System.Collections.Generic;
using Helium.Distributors.Commissions.Incentives;

namespace Helium.Distributors.Commissions.Data.Models
{
    public class IncentiveTieredValueReadModel<T, TK> : IncentiveReadModel, IIncentiveTierValue<T, TK>
    {
        public IncentiveTieredValueReadModel()
        {
            ValueType= IncentiveValueType.Tiered;
        }

        public Dictionary<T, TK> Values { get; set; }
    }
}