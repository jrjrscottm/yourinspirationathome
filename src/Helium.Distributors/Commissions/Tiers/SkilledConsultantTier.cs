using System.Collections.Generic;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers.Qualifications;

namespace Helium.Distributors.Commissions.Tiers
{
    public class SkilledConsultantTier : DefaultCommissionTier
    {
        public SkilledConsultantTier()
            : this(2, .30M, 2,
            new List<IIncentive>
            {
                Tiers.Incentives.FastStartBonusIncentive,
                Tiers.Incentives.FastStartTrainingBonusIncentive,
                Tiers.Incentives.LeadershipBonusIncentive
            })
        {
            //TODO: Remove; only for POC
        }

        public SkilledConsultantTier(int tierId, decimal percentage, int level, IEnumerable<IIncentive> incentives)
            : base(tierId, percentage, level, incentives)
        {
            Qualifications = new List<ICommissionTierQualification>
            {
                new PersonalVolumeCommissionTierQualification(300, 749.99M),
                new GroupVolumeCommissionTierQualification(1000, 1749.99M)
            };
        }
    }
}