namespace Helium.Distributors.Commissions.Tiers.Qualifications
{
    public class GroupVolumeCommissionTierQualification : VolumeCommissionTierQualification
    {
        public override bool IsQualified(Volume volume)
        {
            return volume.GroupRetailVolume >= MinimumAmount
                   && (
                    !MaximumAmount.HasValue ||
                    volume.GroupRetailVolume <= MaximumAmount
                   );
        }

        public GroupVolumeCommissionTierQualification(decimal minimum, decimal? maximum = null) 
            : base(minimum, maximum)
        {
        }
    }
}