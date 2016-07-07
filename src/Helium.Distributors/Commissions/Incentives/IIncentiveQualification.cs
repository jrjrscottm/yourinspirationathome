namespace Helium.Distributors.Commissions.Incentives
{
    public interface IIncentiveQualification
    {
        bool IsQualified(Distributor distributor);
    }
}