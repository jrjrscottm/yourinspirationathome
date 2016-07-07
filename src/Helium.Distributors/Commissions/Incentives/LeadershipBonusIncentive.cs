using System;
using System.Collections.Generic;
using System.Linq;
using Helium.Distributors.Commissions.Data.Models;

namespace Helium.Distributors.Commissions.Incentives
{
    public interface IMatrixIncentive<out T>
    {
        T GetMatrixValue(int tier, int depth);
    }

    public class LeadershipBonusIncentive : IIncentive
    {
        public LeadershipBonusIncentive(
            IIncentiveValueCalculator valueCalculator) 
        {
            ValueType = IncentiveValueType.Matrix;
            ValueCalculator = valueCalculator;
        }

        public class LeadershipBonusValueCalculator : IIncentiveValueCalculator
        {
            public LeadershipBonusValueCalculator(IIncentiveMatrix matrix)
            {
                IncentiveMatrix = matrix.Values;
            }
            
            private decimal[,] IncentiveMatrix { get; }
            //TODO: Cleanup, this was boring to type.
            //= {
            //    {0M,0,0,0,0},
            //    {.05M,0,0,0,0},
            //    {.1M,.03M,0,0,0},
            //    {.1M,.03M,.01M,0,0},
            //    {.1M,.03M,.02M,.01M,0},
            //    {.1M,.04M,.02M,.01M,.005M},
            //    {.1M,.05M,.02M,.01M,.01M}
            //};

            public decimal GetValueAmount(Distributor distributor, Volume volume)
            {
                var startDate = volume.StartDate;
                var endDate = volume.EndDate;

                if (!distributor.Downline.Members.Any())
                    return 0M;

                var maxDepth = GetCommissionTierMaxDepth(distributor);
                var totalCommission = 0M;

                for (var i = 1; i <= maxDepth; i++)
                {
                    var currentDepth = i;
                    totalCommission += GetDownlineMembers(distributor, i)
                        .Sum(member =>
                        {
                            var bonusVolume = member.CalculateVolume(startDate, endDate).BonusVolume;
                            var commissionPercentage =
                                IncentiveMatrix[distributor.Status.CommissionTier.Level, currentDepth];
                            return bonusVolume * commissionPercentage;
                        });
                }

                return totalCommission;
            }

            protected int GetCommissionTierMaxDepth(Distributor distributor)
            {
                switch (distributor.Status.CommissionTier.Level)
                {
                    case 1: return 0;
                    case 2: return 1;
                    case 3: return 2;
                    case 4: return 3;
                    case 5: return 4;
                    case 6:
                    case 7: return 5;
                    default: return 0;
                }
            }

            public IEnumerable<Distributor> GetDownlineMembers(Distributor distributor, int depth)
            {
                if (depth == 0) return Enumerable.Empty<Distributor>();

                //Depth 1
                IEnumerable<Distributor> members = distributor.Downline.Members;

                //Start at Depth 2
                for (var i = 2; i <= depth; i++)
                {
                    members = members.SelectMany(downlineDistributor => 
                        downlineDistributor.Downline.Members);
                }

                return members;
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IncentiveValueType ValueType { get; set; }
        public IIncentiveValueCalculator ValueCalculator { get; }
    }
}