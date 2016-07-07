using System;
using System.Collections.Generic;
using Helium.Distributors.Commissions;
using Helium.Distributors.Commissions.Tiers;

namespace Helium.Distributors.UnitTests
{
    public static class DistributorSamples
    {
        public static Distributor GetConsultantDistributor()
        {
            var distributor = new Distributor
            {
                ActivationDate = DateTime.UtcNow,
                MemberId = IdentityConvention.CreateMemberId(),
                Status = new DistributorStatus
                {
                    IsDeactivated = false,
                    CommissionTier = new ConsultantTier()
                }
            };

            distributor.Downline = new Downline(distributor);

            distributor.AttributedOrders = new List<Order>
            {
                new Order
                {
                    DatePlaced = DateTime.UtcNow.AddDays(-1),
                    SubTotal = 150M,
                    OrderItems = new List<ICommissionableItem>
                    {
                        new Product(150, 75)
                    }
                }
            };

            return distributor;         
        }

        public static Distributor GetSkilledConsultantDistributor()
        {
            var distributor = new Distributor
            {
                ActivationDate = DateTime.UtcNow.AddDays(-20),
                MemberId = IdentityConvention.CreateMemberId(),
                Status = new DistributorStatus
                {
                    IsDeactivated = false,
                    CommissionTier = new ConsultantTier()
                }
            };

            distributor.Downline = new Downline(distributor)
            {
                Members = new List<Distributor>
                {
                    new Distributor
                    {
                        ActivationDate = DateTime.UtcNow.AddDays(-15),
                        MemberId = IdentityConvention.CreateMemberId(),
                        SponsorId = distributor.MemberId,
                        Status = new DistributorStatus
                        {
                            CommissionTier = new ConsultantTier()
                        },
                        AttributedOrders = new List<Order>
                        {
                            new Order
                            {
                                DatePlaced = DateTime.UtcNow.AddDays(-1),
                                SubTotal = 1000M,
                                OrderItems = new List<ICommissionableItem>
                                {
                                    new Product(1000, 1000)
                                }
                            }
                        },
                        Downline = new Downline()
                    }
                }

            };

            distributor.AttributedOrders = new List<Order>
            {
                new Order
                {
                    DatePlaced = DateTime.UtcNow.AddDays(-1),
                    SubTotal = 150M,
                    OrderItems = new List<ICommissionableItem>
                    {
                        new Product(150, 150)
                    }
                },
                new Order
                {
                    DatePlaced = DateTime.UtcNow.AddDays(-1),
                    SubTotal = 150M,
                    OrderItems = new List<ICommissionableItem>
                    {
                        new Product(150, 150)
                    }
                }
            };

            return distributor;
        }
    }
}