namespace Helium.Distributors.Commissions.Incentives
{
    public interface IIncentiveValueCalculator
    {
        decimal GetValueAmount(Distributor distributor, Volume volume);
    }
}