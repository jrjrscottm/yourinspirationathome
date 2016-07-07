namespace Helium.Distributors.Commissions.Tiers.Qualifications
{

    public enum CommissionVolumeType
    {
        Personal,
        Group
    }
    public interface ICommissionTierQualification
    {
        bool IsQualified(object value);
    }
    public interface ICommissionTierQualification<in T> : ICommissionTierQualification
    {
        bool IsQualified(T value);
    }
    public interface IVolumeCommissionTierQualification : ICommissionTierQualification<Volume>
    {
    }

    public interface IDownlineCommisionTierQualification : ICommissionTierQualification<Downline>
    {
    }
}