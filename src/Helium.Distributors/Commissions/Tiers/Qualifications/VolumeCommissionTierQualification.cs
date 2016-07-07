namespace Helium.Distributors.Commissions.Tiers.Qualifications
{
    public abstract class VolumeCommissionTierQualification : IVolumeCommissionTierQualification
    {
        protected VolumeCommissionTierQualification(decimal minimum, decimal? maximum = null)
        {
            MinimumAmount = minimum;
            MaximumAmount = maximum;
        }
        public decimal MinimumAmount { get; set; }
        public decimal? MaximumAmount { get; set; }
        public abstract bool IsQualified(Volume volume);

        bool ICommissionTierQualification.IsQualified(object value)
        {
            var volume = value as Volume;
            return volume != null && IsQualified(volume);
        }
    }
}