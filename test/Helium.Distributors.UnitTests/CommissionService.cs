using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helium.Distributors.Commissions;
using Helium.Distributors.Commissions.Incentives;
using Helium.Distributors.Commissions.Tiers;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Xunit;

namespace Helium.Distributors.UnitTests
{
    public class CommissionService
    {
        private IFixture CreateFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void LoadsTiers_WithCommissionTierService()
        {
            var fixture = CreateFixture();
            var mock = fixture.RegisterMock<ICommissionTierService>();

            var commissionsService = fixture.Create<CommissionsService>();

            var tiers = commissionsService.Tiers;

            Mock.Get(mock).Verify(service => service.GetAll(), Times.Once);
        }

        [Fact]
        public void CalculateCommission_QualifiesTier()
        {
            var fixture = CreateFixture();
            var mock = fixture.RegisterMock<IQualificationsService>();

            var commissionsService = fixture.Create<CommissionsService>();

            commissionsService.CalculateCommissionsPayout(DateTime.UtcNow, DateTime.UtcNow, new Distributor());

            Mock.Get(mock).Verify(service => service.GetHighestQualifiedTier(
                It.IsAny<IEnumerable<ICommissionTier>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()),
                Times.Once);
        }

        [Fact]
        public void CanCalculate_ConsultantPercentageCommissions()
        {
            var fixture = CreateFixture();

            var qualificationsService = fixture.Freeze<Mock<IQualificationsService>>();

            qualificationsService.Setup(
                q =>
                    q.GetHighestQualifiedTier(It.IsAny<IEnumerable<ICommissionTier>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()))
                    .Returns(() => new ConsultantTier());

            var commissions = fixture.Create<CommissionsService>();

            var payout = commissions.CalculateCommissionsPayout(
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow,
                DistributorSamples.GetConsultantDistributor());

            Assert.Equal(18.75M, payout.CommissionablePayoutAmout);
        }

        [Fact]
        public void CanCalculate_SkilledConsultantPercentageCommissions()
        {
            var fixture = CreateFixture();

            var qualificationsService = fixture.Freeze<Mock<IQualificationsService>>();

            qualificationsService.Setup(
                q =>
                    q.GetHighestQualifiedTier(It.IsAny<IEnumerable<ICommissionTier>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()))
                    .Returns(() => new SkilledConsultantTier());

            var commissions = fixture.Create<CommissionsService>();

            var payout = commissions.CalculateCommissionsPayout(
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow,
                DistributorSamples.GetSkilledConsultantDistributor());

            Assert.Equal(90M, payout.CommissionablePayoutAmout);
        }

        [Fact]
        public void CalculateCommissions_GetsQualifiedIncentivesTotal()
        {
            var fixture = CreateFixture();

            var qualificationsService = fixture.Freeze<Mock<IQualificationsService>>();

            qualificationsService.Setup(
                q =>
                    q.GetHighestQualifiedTier(It.IsAny<IEnumerable<ICommissionTier>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()))
                    .Returns(() => fixture.Create<ICommissionTier>());

            var incentiveService = fixture.Freeze<Mock<IIncentiveService>>();

            var commissions = fixture.Create<CommissionsService>();

            var payout = commissions.CalculateCommissionsPayout(
                DateTime.UtcNow,
                DateTime.UtcNow,
                new Distributor());

            incentiveService.Verify(m => m.GetQualifiedIncentiveTotal(
                It.IsAny<IEnumerable<IIncentive>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()
                ), Times.Once);

        }


        public void CalculateCommissions_GetsImplicitIncentivesTotal()
        {
            var fixture = CreateFixture();

            var qualificationsService = fixture.Freeze<Mock<IQualificationsService>>();

            qualificationsService.Setup(
                q =>
                    q.GetHighestQualifiedTier(It.IsAny<IEnumerable<ICommissionTier>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()))
                    .Returns(() => fixture.Create<ICommissionTier>());

            var incentiveService = fixture.Freeze<Mock<IIncentiveService>>();

            var commissions = fixture.Create<CommissionsService>();

            var payout = commissions.CalculateCommissionsPayout(
                DateTime.UtcNow,
                DateTime.UtcNow,
                new Distributor());

            incentiveService.Verify(m => m.GetImplicitIncentiveTotal(
                It.IsAny<IEnumerable<IIncentive>>(), It.IsAny<Distributor>(), It.IsAny<Volume>()
                ), Times.Once);

        }
    }

    public static class FixtureExtensions
    {
        public static T RegisterMock<T>(this IFixture fixture)
            where T: class
        {
            var mock = Mock.Of<T>();
            fixture.Register(() => mock);
            return mock;
        }
    }
}
