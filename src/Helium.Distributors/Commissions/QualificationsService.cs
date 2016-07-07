using System.Collections.Generic;
using System.Linq;
using Helium.Distributors.Commissions.Data;
using Helium.Distributors.Commissions.Tiers;
using Helium.Distributors.Commissions.Tiers.Qualifications;

namespace Helium.Distributors.Commissions
{
    public interface IQualificationsService
    {
        IEnumerable<ICommissionTierQualification> GetCommissionTierQualifications(int commissionTierId);

        ICommissionTier GetHighestQualifiedTier(
            IEnumerable<ICommissionTier> commissionTiers, Distributor distributor,Volume volume);
    }

    public class QualificationsService : IQualificationsService
    {
        private readonly IQualificationRepository _qualificationRepository;
        public QualificationsService(IQualificationRepository qualificationRepository)
        {
            _qualificationRepository = qualificationRepository;
        }

        public IEnumerable<ICommissionTierQualification> GetCommissionTierQualifications(int commissionTierId)
        {
            var tierQualifications = _qualificationRepository.GetVolumeQualificationsByCommissionTierId(commissionTierId);

            foreach (var qualification in tierQualifications)
            {
                switch (qualification.VolumeType)
                {
                    case CommissionVolumeType.Personal:
                        yield return 
                            new PersonalVolumeCommissionTierQualification(qualification.MinimumVolume, qualification.MaximumVolume);
                        break;
                    case CommissionVolumeType.Group:
                        yield return 
                            new GroupVolumeCommissionTierQualification(qualification.MinimumVolume, qualification.MaximumVolume);
                        break;
                    default:
                        continue;
                }
            }
        }

        public ICommissionTier GetHighestQualifiedTier(
            IEnumerable<ICommissionTier> commissionTiers,Distributor distributor, Volume volume)
        {
            var highestTier = commissionTiers.Where(tier =>
                    tier.Qualifications.All(qualification => qualification.IsQualified(distributor) || qualification.IsQualified(volume)))
                    .OrderByDescending(t => t.Level).FirstOrDefault();

            return highestTier;
        }
    }
}
