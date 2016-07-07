namespace Helium.Distributors.Commissions
{
    public interface IDistributorService
    {
        DistributorStatus GetDistributorStatus(Distributor distributor);
    }
}