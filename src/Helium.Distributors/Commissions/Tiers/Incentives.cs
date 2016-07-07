using System.Collections.Generic;
using Helium.Distributors.Commissions.Data.Models;
using Helium.Distributors.Commissions.Incentives;

namespace Helium.Distributors.Commissions.Tiers
{
    public static class Incentives
    {
        public static IIncentive FastStartBonusIncentive = new FastStartBonusQualifiedIncentive(
            new FastStartBonusQualifiedIncentive.FastStartBonusValueCalculator(
                    new IncentiveTieredValueReadModel<int, decimal>
                    {
                        Id = 1,
                        Name = "Fast Start Bonus",
                        Description = "Fast Start Bonus <copy>",
                        Values = new Dictionary<int, decimal>
                        {
                            {20, 50M },
                            {30, 100M },
                            {40, 100M },
                            {50, 100M },
                            {60, 125M },
                            {70, 150M },
                        }
                    }
                ),
            new List<IIncentiveQualification>
            {
                new FastStartBonusQualifiedIncentive.FastStartBonusQualification()
            }
        );

        public static IIncentive FastStartTrainingBonusIncentive = new FastStartBonusQualifiedIncentive(
            new FastStartBonusQualifiedIncentive.FastStartBonusValueCalculator(
                new IncentiveTieredValueReadModel<int, decimal>
                {
                    Id = 2,
                    Name = "Fast Start Training Bonus",
                    Description = "Fast Start Training Bonus <copy>",
                    Values = new Dictionary<int, decimal>
                        {
                            {20, 50M },
                            {30, 100M },
                            {40, 100M },
                            {50, 100M },
                            {60, 125M },
                            {70, 150M },
                        }
                }),
            new List<IIncentiveQualification>
            {
                new FastStartBonusQualifiedIncentive.FastStartTrainingBonusQualification()
            }
        );

        public static IIncentive LeadershipBonusIncentive = new LeadershipBonusIncentive(
            new LeadershipBonusIncentive.LeadershipBonusValueCalculator(
                new IncentiveMatrixValueReadModel(
                    new List<List<decimal>>
                    {
                        new List<decimal>{0M,0M,0M,0M,0M},
                        new List<decimal>{0.05M,0M,0M,0M,0M},
                        new List<decimal>{0.10M,0.03M,0M,0M,0M},
                        new List<decimal>{0.10M,0.03M,0M,0M,0M},
                        new List<decimal>{0.10M,0.03M,0.10M,0M,0M},
                        new List<decimal>{0.10M,0.04M,0.10M,0M,0.005M},
                        new List<decimal>{0.10M,0.05M,0.10M,0M,0.01M}


                    }, 5)
                ));
    }
}