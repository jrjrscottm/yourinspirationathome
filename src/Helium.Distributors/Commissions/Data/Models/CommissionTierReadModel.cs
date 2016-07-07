using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helium.Distributors.Commissions.Tiers;

namespace Helium.Distributors.Commissions.Data.Models
{
    public class CommissionTierReadModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public decimal CommissionPercentage { get; set; }
    }

    public static class CommissionTierReadModelExtensions
    {
        public static DefaultCommissionTier ToDefaultCommissionTier(this CommissionTierReadModel model)
        {
            return new DefaultCommissionTier(model.Id, model.Name, model.CommissionPercentage, model.Level);
        }
    }
}
