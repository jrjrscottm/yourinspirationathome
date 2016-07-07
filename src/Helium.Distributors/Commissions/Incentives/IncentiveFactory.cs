using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helium.Distributors.Commissions.Data.Models;

namespace Helium.Distributors.Commissions.Incentives
{

    public interface IIncentiveFactory
    {
        IIncentive CreateIncentiveMatrixInstance(int id, string name, string description, IIncentiveMatrix matrix);
        IIncentive CreateIncentiveTieredInstance(int id, string name, string description, IIncentiveTierValue<int, decimal> values);
    }
    public class IncentiveFactory : IIncentiveFactory
    {
        public IIncentive CreateIncentiveMatrixInstance(
            int id, string name, string description, IIncentiveMatrix matrix)
        {
            switch (name)
            {
                case "Leadership Bonus":
                    return new LeadershipBonusIncentive(
                        new LeadershipBonusIncentive.LeadershipBonusValueCalculator(matrix)
                    )
                    {
                        Id = id,
                        Name = name,
                        Description = description
                    };
                default:
                    throw new ArgumentException("Unknown incentive type", name);
            }
        }

        public IIncentive CreateIncentiveTieredInstance(
            int id, string name, string description, IIncentiveTierValue<int, decimal> values)
        {
            IIncentive incentive;
            switch (name)
            {
                case "Fast Start Bonus":
                    {   incentive = new FastStartBonusQualifiedIncentive(
                        new FastStartBonusQualifiedIncentive.FastStartBonusValueCalculator(values),
                        new List<IIncentiveQualification>
                        {
                            new FastStartBonusQualifiedIncentive.FastStartBonusQualification()
                        });
                    }
                    break;
                case "Fast Start Training Bonus":
                    {
                        incentive = new FastStartBonusQualifiedIncentive(
                        new FastStartBonusQualifiedIncentive.FastStartBonusValueCalculator(values),
                        new List<IIncentiveQualification>
                        {
                            new FastStartBonusQualifiedIncentive.FastStartTrainingBonusQualification()
                        });
                    }
                    break;
                default:
                    throw new ArgumentException("Unknown incentive type", name);
            }

            incentive.Id = id;
            incentive.Name = name;
            incentive.Description = description;

            return incentive;
        }
    }
}
