namespace Helium.Distributors.Commissions.Incentives
{
    public interface IIncentive
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        IncentiveValueType ValueType { get; set; }

        IIncentiveValueCalculator ValueCalculator { get; }
    }
}