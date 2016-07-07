using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Helium.Distributors.Commissions.Data;
using Helium.Distributors.Commissions.Data.Models;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers;

namespace Helium.Distributors.Commissions
{
    public interface ICommissionsService
    {
        IEnumerable<ICommissionTier> Tiers { get; }
        Payout CalculateCommissionsPayout(DateTime startDate, DateTime endDate, Distributor distributor);
    }

    public interface ICommissionTierRepository
    {
        IEnumerable<CommissionTierReadModel> GetAllTiers();
    }

    public class CommissionTierRepository : ICommissionTierRepository
    {
        private readonly IRepositoryConnectionFactory _connectionFactory;

        public CommissionTierRepository(IRepositoryConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<CommissionTierReadModel> GetAllTiers()
        {
            using (var connection = _connectionFactory.CreateOpenConnection())
            {
                return connection.Query<CommissionTierReadModel>(
                    "CommissionTier_GetAll",
                    commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }

    public interface ICommissionTierService
    {
        IEnumerable<ICommissionTier> GetAll();
    }

    public class CommissionTierService : ICommissionTierService
    {
        private readonly ICommissionTierRepository _commissionTierRepository;
        private readonly IQualificationsService _qualificationService;
        private readonly IIncentiveService _incentiveService;

        public CommissionTierService(
            ICommissionTierRepository commissionTierRepository,
            IQualificationsService qualificationService,
            IIncentiveService incentiveService)
        {
            _commissionTierRepository = commissionTierRepository;
            _qualificationService = qualificationService;
            _incentiveService = incentiveService;
        }

        public IEnumerable<ICommissionTier> GetAll()
        {
            var commissionTierModels = _commissionTierRepository.GetAllTiers();

            var commissionTiers = new List<ICommissionTier>();
            foreach (var commissionTierModel in commissionTierModels)
            {
                var commissionTier = commissionTierModel.ToDefaultCommissionTier();

                commissionTier.SetQualifications(
                    _qualificationService.
                    GetCommissionTierQualifications(commissionTier.CommissionTierId));

                commissionTier.SetIncentives(
                    _incentiveService
                        .GetIncentivesByCommissionTierId(commissionTier.CommissionTierId));

                commissionTiers.Add(commissionTier);
            }


            return commissionTiers;
        }
    }


    public class CommissionsService : ICommissionsService
    {
        private readonly ICommissionTierService _commissionTierService;
        private readonly IQualificationsService _qualificationsService;
        private readonly IIncentiveService _incentivesService;
        private IEnumerable<ICommissionTier> _tiers;

        public CommissionsService(
            ICommissionTierService commissionTierService,
            IQualificationsService qualificationsService,
            IIncentiveService incentivesService)
        {
            _commissionTierService = commissionTierService;
            _qualificationsService = qualificationsService;
            _incentivesService = incentivesService;
        }

        public IEnumerable<ICommissionTier> Tiers => _tiers ?? (_tiers = _commissionTierService.GetAll());

        public Payout CalculateCommissionsPayout(DateTime startDate, DateTime endDate, Distributor distributor)
        {
            var volume = distributor.CalculateVolume(startDate, endDate);

            var highestTier = _qualificationsService.GetHighestQualifiedTier(Tiers, distributor, volume);

            if (highestTier == null) return null;

            distributor.Status.CommissionTier = highestTier;

            var commissions = volume.PersonalCommissionableVolume*highestTier.CommissionPercentage;

            var totalQualifiedIncentive = _incentivesService
                .GetQualifiedIncentiveTotal(
                    distributor.Status.CommissionTier.Incentives, distributor, volume);

            var totalImplicitIncentive = _incentivesService
                .GetImplicitIncentiveTotal(
                    distributor.Status.CommissionTier.Incentives, distributor, volume);

            return new Payout
            {
                CommissionablePayoutAmout = commissions,
                IncentivePayoutAmount = totalQualifiedIncentive + totalImplicitIncentive
            };
        }


    }
}