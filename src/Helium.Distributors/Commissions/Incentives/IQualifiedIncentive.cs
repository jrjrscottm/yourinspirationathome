using System;

namespace Helium.Distributors.Commissions.Incentives
{
    public interface IQualifiedIncentive : IIncentive
    {       
        decimal GetQualifiedCommission(Distributor distributor, Volume volume);

    }
}