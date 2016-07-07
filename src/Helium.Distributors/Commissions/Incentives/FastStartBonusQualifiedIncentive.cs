using System;
using System.Collections.Generic;
using System.Linq;
using Helium.Distributors.Commissions.Data.Models;

namespace Helium.Distributors.Commissions.Incentives
{
    public class FastStartBonusQualifiedIncentive : QualifiedIncentiveBase
    {
        public FastStartBonusQualifiedIncentive(
            IIncentiveValueCalculator valueCalculator,
            IEnumerable<IIncentiveQualification> qualifiers)
            :base(valueCalculator, qualifiers)
        {
            ValueType = IncentiveValueType.Tiered;
        }

        public class FastStartBonusQualification : IIncentiveQualification
        {
            public virtual bool IsQualified(Distributor distributor)
            {

                var day30total = distributor.CalculateVolume(distributor.ActivationDate,
                    distributor.ActivationDate.AddDays(30))
                    .PersonalRetailVolume;
                var day60total = 
                distributor.CalculateVolume(distributor.ActivationDate, distributor.ActivationDate.AddDays(60))
                    .PersonalRetailVolume;


                //TODO: Configurable Properties
                return !distributor.Status.IsDeactivated
                       && distributor.ActivationDate >= DateTime.UtcNow.AddDays(-60)
                       && day30total >= 450
                       && day60total >=900;
                        
                //distributor.CalculateVolume(distributor.ActivationDate, distributor.ActivationDate.AddDays(30))
                //    .PersonalRetailVolume >= 450
                //&&
                //distributor.CalculateVolume(distributor.ActivationDate, distributor.ActivationDate.AddDays(60))
                //    .PersonalRetailVolume >= 900;
            }
        }

        public class FastStartTrainingBonusQualification : FastStartBonusQualification
        {
            public override bool IsQualified(Distributor distributor)
            {
                var personallySponsored = distributor.Downline.Members.Where(x => 
                    !x.Status.IsDeactivated 
                    && x.SponsorId == distributor.MemberId);

                return personallySponsored.Any(member =>
                    base.IsQualified(member)
                    );
            }
        }

        public class FastStartBonusValueCalculator
            : IIncentiveValueCalculator
        {
            private readonly IIncentiveTierValue<int, decimal> _tierValues;

            public FastStartBonusValueCalculator(IIncentiveTierValue<int, decimal> tierValues)
            {
                _tierValues = tierValues;
            }

            public decimal GetValueAmount(Distributor distributor, Volume volume)
            {
                return _tierValues.Values[distributor.Status.CommissionTier.CommissionTierId];
                //TODO: Configurable Properties
            }
        }
    }
}