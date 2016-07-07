using System;
using System.Collections.Generic;
using System.Linq;
using Helium.Distributors.Commissions.Tiers;

namespace Helium.Distributors
{
    public class Distributor
    {
        public string MemberId { get; set; }
        public string SponsorId { get; set; }
        public DistributorStatus Status { get; set; } = new DistributorStatus();
        public DateTime ActivationDate { get; set; }
        public Downline Downline { get; set; } = new Downline();
        public List<Order> AttributedOrders { get; set; } = new List<Order>();
        public Volume CalculateVolume(DateTime startDate, DateTime endDate)
        {
            var volume = new Volume(startDate, endDate);

            var periodOrders = AttributedOrders.Where(order => order.DatePlaced >= startDate && order.DatePlaced <= endDate).ToList();

            periodOrders.ForEach(order =>
            {
                volume.PersonalRetailVolume += order.SubTotal;
                volume.PersonalCommissionableVolume += order.OrderItems.Sum(x => x.CommissionableValue);
                volume.BonusVolume = volume.PersonalRetailVolume*.65M;
            });

            var personallySponsoredOrders = Downline.Members
                .Where(member => member.SponsorId == MemberId && member.Status.IsDeactivated == false) // Personally sponsored
                .SelectMany(member =>
                    member.AttributedOrders.Where(order => order.DatePlaced >= startDate && order.DatePlaced <= endDate)).ToList();

            personallySponsoredOrders.ForEach(order =>
            {
                volume.GroupRetailVolume += order.SubTotal;
                volume.GroupCommissionableVolume += order.OrderItems.Sum(x => x.CommissionableValue);
            });

            return volume;
        }
    }

    public class DistributorStatus
    {
        public ICommissionTier CommissionTier { get; set; }
        public bool IsDeactivated { get; set; }

    }
}

