using System.Collections.Generic;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers.Qualifications;
using System.Linq;

namespace Helium.Distributors.Commissions.Tiers
{

    public class DefaultCommissionTier : ICommissionTier
    {
        public DefaultCommissionTier(int tierId, string name, decimal percentage, int level)
        {
            CommissionTierId = tierId;
            Name = name;
            CommissionPercentage = percentage;
            Level = level;
        }

        protected DefaultCommissionTier(int tierId, decimal percentage, int level, IEnumerable<IIncentive> incentives)
        {
            CommissionTierId = tierId;
            CommissionPercentage = percentage;
            Level = level;
            Incentives = incentives.ToList();
        }

        public int CommissionTierId { get; }
        public string Name { get; }
        public decimal CommissionPercentage { get; }
        public int Level { get; }
        public IReadOnlyCollection<ICommissionTierQualification> Qualifications { get; protected set; }
        public IReadOnlyCollection<IIncentive> Incentives { get; protected set; }

        public virtual void SetQualifications(IEnumerable<ICommissionTierQualification> qualifications)
        {
            Qualifications = qualifications.ToList().AsReadOnly();
        }

        public virtual void SetIncentives(IEnumerable<IIncentive> incentives)
        {
            Incentives = incentives.ToList().AsReadOnly();
        }
    }
}
