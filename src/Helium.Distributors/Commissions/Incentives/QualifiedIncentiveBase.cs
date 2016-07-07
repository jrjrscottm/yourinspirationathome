using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Helium.Distributors.Commissions.Incentives
{
    public abstract class QualifiedIncentiveBase : IQualifiedIncentive
    {
        protected QualifiedIncentiveBase(
            IIncentiveValueCalculator valueCalculator,
            IEnumerable<IIncentiveQualification> qualifiers)
        {
            ValueCalculator = valueCalculator;
            Qualifiers = new ReadOnlyCollection<IIncentiveQualification>(qualifiers.ToList());
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IncentiveValueType ValueType { get; set; }
        public IIncentiveValueCalculator ValueCalculator { get; protected set; }
        public IReadOnlyCollection<IIncentiveQualification> Qualifiers { get; protected set; }

        protected virtual bool IsQualified(Distributor distributor)
        {
            return Qualifiers.All(qualifier => qualifier.IsQualified(distributor));
        }

        public virtual decimal GetQualifiedCommission(Distributor distributor, Volume volume)
        {
            return IsQualified(distributor) 
                ? ValueCalculator.GetValueAmount(distributor, volume)
                : 0M;
        }
    }
}