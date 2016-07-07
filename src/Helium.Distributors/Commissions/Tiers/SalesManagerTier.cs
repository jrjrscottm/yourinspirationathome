using System.Collections.Generic;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers.Qualifications;

namespace Helium.Distributors.Commissions.Tiers
{
    public class SalesManagerTier : DefaultCommissionTier
    {
        public SalesManagerTier()
            : this(3, .35M, 3,
            new List<IIncentive>
            {
                Tiers.Incentives.FastStartBonusIncentive,
                Tiers.Incentives.FastStartTrainingBonusIncentive,
                Tiers.Incentives.LeadershipBonusIncentive
            })
        {
            
        }

        public SalesManagerTier(int tierId, decimal percentage, int level, IEnumerable<IIncentive> incentives)
            : base(tierId, percentage, level, incentives)
        {
            Qualifications = new List<ICommissionTierQualification>
            {
                new PersonalVolumeCommissionTierQualification(750, 999.99M),
                new GroupVolumeCommissionTierQualification(1750, 3999.99M)
            };
        }
    }
}