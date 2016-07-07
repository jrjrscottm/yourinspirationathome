namespace Helium.Distributors.Commissions
{
    public interface ICommissionableItem
    {
        decimal SuggestedRetailValue { get; set; }
        decimal CommissionableValue { get; set; }
        decimal CommissionableValuePercentage { get; set; }
    }

    public class Product : ICommissionableItem
    {
        public Product()
        {
            
        }

        public Product(decimal suggestedRetail, decimal commissionableValue)
        {
            SuggestedRetailValue = suggestedRetail;
            CommissionableValue = commissionableValue;
        }

        public decimal SuggestedRetailValue { get; set;  }
        public decimal CommissionableValue { get; set; }
        public decimal CommissionableValuePercentage { get; set; }
    }
}