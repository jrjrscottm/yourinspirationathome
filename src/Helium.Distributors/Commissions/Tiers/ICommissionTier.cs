using System.Collections.Generic;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers.Qualifications;

namespace Helium.Distributors.Commissions.Tiers
{
    public interface ICommissionTier
    {
        int CommissionTierId { get; }
        string Name { get; }
        decimal CommissionPercentage { get; }
        int Level { get; }
        
        IReadOnlyCollection<ICommissionTierQualification> Qualifications { get; }
        IReadOnlyCollection<IIncentive> Incentives { get; }
    }
}