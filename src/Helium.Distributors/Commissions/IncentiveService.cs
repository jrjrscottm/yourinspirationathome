using System;
using System.Collections.Generic;
using System.Threading;
using Helium.Distributors.Commissions.Data;
using Helium.Distributors.Commissions.Incentives;
using System.Linq;

namespace Helium.Distributors.Commissions
{
    public interface IIncentiveService
    {
        IIncentive GetIncentive(int incentiveId);
        IEnumerable<IIncentive> GetIncentivesByCommissionTierId(int commissionTierId);

        decimal GetQualifiedIncentiveTotal(IEnumerable<IIncentive> incentives, Distributor distributor, Volume volume);
        decimal GetImplicitIncentiveTotal(IEnumerable<IIncentive> incentives, Distributor distributor, Volume volume);
    }


    public class IncentiveService : IIncentiveService
    {
        private readonly IIncentiveRepository _incentiveRepository;
        private readonly IIncentiveFactory _incentiveFactory;

        public IncentiveService(
            IIncentiveRepository incentiveRepository,
            IIncentiveFactory incentiveFactory)
        {
            _incentiveRepository = incentiveRepository;
            _incentiveFactory = incentiveFactory;
            _incentiveLoader = new Lazy<Dictionary<int, IIncentive>>(GetIncentives);
        }

        public IIncentive GetIncentive(int incentiveId)
        {
            return Incentives[incentiveId];
        }

        public IEnumerable<IIncentive> GetIncentivesByCommissionTierId(int commissionTierId)
        {
            var commissionTierIncentives = _incentiveRepository.GetIncentivesByCommissionTierId(commissionTierId);
            foreach (var incentive in commissionTierIncentives)
            {
                yield return GetIncentive(incentive.Id);
            }
        }

        public decimal GetQualifiedIncentiveTotal(IEnumerable<IIncentive> incentives, Distributor distributor, Volume volume)
        {
            return incentives.OfType<IQualifiedIncentive>()
                .Sum(qualified => qualified.GetQualifiedCommission(distributor, volume));
        }

        public decimal GetImplicitIncentiveTotal(IEnumerable<IIncentive> incentives, Distributor distributor, Volume volume)
        {
            return incentives.Where(incentive => !(incentive is IQualifiedIncentive))
                .Sum(x => x.ValueCalculator.GetValueAmount(distributor, volume));
        }

        //TODO: Worry about caching/db updates later. Can manage lifetime in container.
        private readonly Lazy<Dictionary<int, IIncentive>> _incentiveLoader;
        private Dictionary<int, IIncentive> Incentives => _incentiveLoader.Value;

        private Dictionary<int, IIncentive> GetIncentives()
        {
            var incentives = new Dictionary<int, IIncentive>();
              var incentiveModels = _incentiveRepository.GetAllIncentives();

            foreach (var incentive in incentiveModels)
            {
                switch (incentive.ValueType)
                {
                    case IncentiveValueType.Matrix:
                        {
                            var matrix = _incentiveRepository.GetIncentiveMatrixValues(incentive.Id);

                            Incentives[incentive.Id] =
                                _incentiveFactory.CreateIncentiveMatrixInstance(
                                    incentive.Id, incentive.Name, incentive.Description, matrix);
                        }
                        break;
                    case IncentiveValueType.Tiered:
                        {
                            var tierValues = _incentiveRepository.GetIncentiveTieredValues(incentive.Id);

                            Incentives[incentive.Id] =
                                _incentiveFactory.CreateIncentiveTieredInstance(
                                    incentive.Id, incentive.Name, incentive.Description, tierValues);
                        }
                        break;
                }
            }

            return incentives;
        }
    }
}