using System.Linq;
using System.Threading.Tasks;
using Helium.Distributors.Commissions.Incentives;

namespace Helium.Distributors.Commissions.Data.Models
{
    public class IncentiveReadModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IncentiveValueType ValueType { get; set; }
    }
}
