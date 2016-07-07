using System.Collections.Generic;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers.Qualifications;

namespace Helium.Distributors.Commissions.Tiers
{
    public class ConsultantTier : DefaultCommissionTier
    {
        public ConsultantTier()
            : this(1, .25M, 1, new List<IIncentive>())
        {
            
        }

        public ConsultantTier(int tierId, decimal percentage, int level, IEnumerable<IIncentive> incentives)
            : base(tierId, percentage, level, incentives)
        {
            Qualifications = new List<ICommissionTierQualification>
            {
                new PersonalVolumeCommissionTierQualification(150, 299.99M)
            };
        }
    }
}