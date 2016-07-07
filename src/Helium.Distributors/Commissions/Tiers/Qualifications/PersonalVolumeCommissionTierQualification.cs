namespace Helium.Distributors.Commissions.Tiers.Qualifications
{
    public class PersonalVolumeCommissionTierQualification : VolumeCommissionTierQualification
    {
        public override bool IsQualified(Volume volume)
        {
            return volume.PersonalRetailVolume >= MinimumAmount
                   && (
                    !MaximumAmount.HasValue ||
                    volume.PersonalRetailVolume <= MaximumAmount
                   );
        }

        public PersonalVolumeCommissionTierQualification(decimal minimum, decimal? maximum = null) 
            : base(minimum, maximum)
        {
        }
    }
}