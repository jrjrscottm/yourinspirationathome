using System.Collections.Generic;
using Helium.Distributors.Commissions;
using Helium.Distributors.Commissions.Data;
using Helium.Distributors.Commissions.Data.Models;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers;
using Helium.Distributors.Commissions.Tiers.Qualifications;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Helium.Distributors.UnitTests
{
    public static class FixtureFactory
    {
        public static List<CommissionTierReadModel> CommissionTiers =
            new List<CommissionTierReadModel>
                {
                    new CommissionTierReadModel {Id = 10, Name = "Consultant", Level = 1, CommissionPercentage = .25M},
                    new CommissionTierReadModel {Id = 20, Name = "Skilled Consultant", Level = 2, CommissionPercentage = .30M},
                    new CommissionTierReadModel {Id = 30, Name = "Sales Manager", Level = 3, CommissionPercentage = .35M}
                };


        public static IFixture CreateFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var commissionRepository = new Mock<ICommissionsRepository>();
            commissionRepository.Setup(m => m.GetTiers()).Returns(
                CommissionTiers
                );
            fixture.Register(() => commissionRepository.Object);

            var incentiveService = new Mock<IIncentiveService>();

            incentiveService.Setup(m => m.GetIncentivesByCommissionTierId(
                It.Is<int>(x => x == 10)))
                .Returns(
                    new List<IIncentive>());

            incentiveService.Setup(m => m.GetIncentivesByCommissionTierId(
                It.Is<int>(x => x > 10)))
                .Returns(
                    new List<IIncentive>
                    {
                        Incentives.FastStartBonusIncentive,
                        Incentives.FastStartTrainingBonusIncentive,
                        Incentives.LeadershipBonusIncentive
                    });

            fixture.Register(() => incentiveService.Object);

            var qualificationService = fixture.Freeze<Mock<IQualificationsService>>();

            qualificationService.Setup(m =>
                m.GetCommissionTierQualifications(It.Is<int>(v => v == 10)))
                .Returns(
                    new List<ICommissionTierQualification>
                    {
                        new PersonalVolumeCommissionTierQualification(150M)
                    }
                );

            qualificationService.Setup(m =>
                m.GetCommissionTierQualifications(It.Is<int>(v => v == 20)))
                .Returns(
                    new List<ICommissionTierQualification>
                    {
                        new PersonalVolumeCommissionTierQualification(300M),
                        new GroupVolumeCommissionTierQualification(1000M)
                    }
                );

            qualificationService.Setup(m =>
                m.GetCommissionTierQualifications(It.Is<int>(v => v == 30)))
                .Returns(
                    new List<ICommissionTierQualification>
                    {
                        new PersonalVolumeCommissionTierQualification(750M),
                        new GroupVolumeCommissionTierQualification(1750M)
                    }
                );

            return fixture;
        }
    }
}