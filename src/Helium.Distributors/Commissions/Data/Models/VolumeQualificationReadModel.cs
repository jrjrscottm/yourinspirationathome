using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helium.Distributors.Commissions.Tiers.Qualifications;

namespace Helium.Distributors.Commissions.Data.Models
{
    public class VolumeQualificationReadModel
    {
        public int CommissionTierId { get; set; }
        public int CommissionTierQualificationId { get; set; }
        public CommissionVolumeType VolumeType{ get; set; }
        public decimal MinimumVolume { get; set; }
        public decimal MaximumVolume { get; set; }
    }
}
